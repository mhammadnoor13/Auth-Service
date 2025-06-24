# ------------------------------------------------------------
# 1) Runtime base image
# ------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

# ------------------------------------------------------------
# 2) Build stage
# ------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 2.1) Shared Contracts csproj for ProjectReference
COPY ["Common/Contracts/Contracts.Shared/Contracts.Shared.csproj", "Common/Contracts/Contracts.Shared/"]

# 2.2) Copy AuthService projects for fast restore
COPY ["Services/Auth-Service/src/AuthService.Api/AuthService.Api.csproj",       "Services/Auth-Service/src/AuthService.Api/"]
COPY ["Services/Auth-Service/src/AuthService.Application/AuthService.Application.csproj", "Services/Auth-Service/src/AuthService.Application/"]
COPY ["Services/Auth-Service/src/AuthService.Domain/AuthService.Domain.csproj", "Services/Auth-Service/src/AuthService.Domain/"]
COPY ["Services/Auth-Service/src/AuthService.Infrastructure/AuthService.Infrastructure.csproj", "Services/Auth-Service/src/AuthService.Infrastructure/"]

# 2.3) Restore (will pull in Contracts.Shared)
RUN dotnet restore "Services/Auth-Service/src/AuthService.Api/AuthService.Api.csproj"

# 2.4) Copy all source (AuthService.Api + its dependencies + shared contracts)
COPY . .

# 2.5) Publish from the API project folder
WORKDIR "/src/Services/Auth-Service/src/AuthService.Api"
RUN dotnet publish "AuthService.Api.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

# ------------------------------------------------------------
# 3) Final runtime image
# ------------------------------------------------------------
FROM base AS final
WORKDIR /app

# 3.1) Bring in the wait-for-rabbit script
USER root
COPY ["scripts/wait-for-rabbit.sh", "/wait-for-rabbit.sh"]
RUN chmod +x /wait-for-rabbit.sh
# drop back to the app user
USER $APP_UID

# 3.2) Copy the published output
COPY --from=build /app/publish .

# 3.3) Launch only after RabbitMQ is ready
ENTRYPOINT ["/wait-for-rabbit.sh", "rabbitmq", "dotnet", "AuthService.Api.dll"]
