using AuthService.Api.Consumers;
using AuthService.Application.Auth;
using AuthService.Application.Common.Interfaces;
using AuthService.Application.UseCases;
using AuthService.Infrastructure.Persistence;
using AuthService.Infrastructure.Security;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection("Jwt"));


builder.Services.AddSingleton<IMongoClient>(_ =>
    new MongoClient(builder.Configuration.GetConnectionString("Mongo")));
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IMongoClient>().GetDatabase("AuthDb"));

builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.SetKebabCaseEndpointNameFormatter();
    busConfigurator.AddConsumer<CreateUserConsumer>();
    busConfigurator.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri(builder.Configuration["MessageBroker:Host"]!), h =>
        {
            h.Username(builder.Configuration["MessageBroker:Username"]);
            h.Password(builder.Configuration["MessageBroker:Password"]);
        });

        cfg.ConfigureEndpoints(context);          
    });
});

builder.Services
    .AddScoped<IUserRepository, MongoUserRepository>()
    .AddScoped<IPasswordHashProvider, AspNetPasswordHashProvider>()
    .AddScoped<IJwtTokenProvider, JwtTokenProviderHandle>()
    .AddScoped<IUnitOfWork, MongoUnitOfWork>()
    .AddScoped<IAuthService, AuthService.Application.Auth.AuthService>()
    .AddScoped<ICreateUserUseCase,CreateUserUseCase>()
    .AddScoped<IJwtTokenHandler, JwtTokenHandler>();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      .AddJwtBearer(opts =>
      {
          var cfg = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()!;
          opts.TokenValidationParameters = new TokenValidationParameters
          {
              ValidateIssuer = true,
              ValidateAudience = true,
              ValidateLifetime = true,
              ValidateIssuerSigningKey = true,
              ValidIssuer = cfg.Issuer,
              ValidAudience = cfg.Audience,
              IssuerSigningKey = new SymmetricSecurityKey(
                                  Encoding.UTF8.GetBytes(cfg.Key)),
              ClockSkew = TimeSpan.FromSeconds(30)
          };
      });

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.\r\n\r\n" +
                  "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
                  "Example: \"Bearer eyJhbGciOiJIUzI1Ni…\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                },
                Scheme = "Bearer",
                Name   = "Authorization",
                In     = ParameterLocation.Header,
            },
            Array.Empty<string>()
        }
    });
}
);

builder.Host.UseSerilog((ctx, lc) =>
    lc.ReadFrom.Configuration(ctx.Configuration));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health/ready");

app.Run();
