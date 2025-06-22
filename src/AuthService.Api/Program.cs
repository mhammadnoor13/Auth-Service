using AuthService.Api.Consumers;
using AuthService.Application.Auth;
using AuthService.Application.Common.Interfaces;
using AuthService.Application.UseCases;
using AuthService.Infrastructure.Persistence;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
/*
// 1️⃣ options
builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection("Jwt"));
*/

// 2️⃣ Mongo
builder.Services.AddSingleton<IMongoClient>(_ =>
    new MongoClient(builder.Configuration.GetConnectionString("Mongo")));
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IMongoClient>().GetDatabase("AuthDb"));

// 3️⃣ MassTransit + Mongo outbox
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

        cfg.ConfigureEndpoints(context);          // auto-create endpoints
    });
});

// 4️⃣ DI for app & infra
builder.Services
    .AddScoped<IUserRepository, MongoUserRepository>()
    .AddScoped<IPasswordHashProvider, AspNetPasswordHashProvider>()
    .AddScoped<IJwtTokenProvider, JwtTokenProvider>()
    .AddScoped<IUnitOfWork, MongoUnitOfWork>()
    .AddScoped<IAuthService, AuthService.Application.Auth.AuthService>()
    .AddScoped< ICreateUserUseCase,CreateUserUseCase>();

/*
// 5️⃣ auth / authz
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
      });*/

builder.Services.AddAuthorization();

// 6️⃣ misc
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseSerilog((ctx, lc) =>
    lc.ReadFrom.Configuration(ctx.Configuration));

// ---- Build & run ----
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health/ready");

app.Run();
