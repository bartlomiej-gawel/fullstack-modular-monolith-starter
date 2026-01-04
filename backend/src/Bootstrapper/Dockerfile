FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# Bootstrapper
COPY ["src/Bootstrapper/Bootstrapper.csproj", "src/Bootstrapper/"]
# Common
COPY ["src/Common/Common.Abstractions/Common.Abstractions.csproj", "src/Common/Common.Abstractions/"]
COPY ["src/Common/Common.Infrastructure/Common.Infrastructure.csproj", "src/Common/Common.Infrastructure/"]
# Identity
COPY ["src/Modules/Identity/Modules.Identity/Modules.Identity.csproj", "src/Modules/Identity/Modules.Identity/"]
COPY ["src/Modules/Identity/Modules.Identity.IntegrationEvents/Modules.Identity.IntegrationEvents.csproj", "src/Modules/Identity/Modules.Identity.IntegrationEvents/"]

RUN dotnet restore "src/Bootstrapper/Bootstrapper.csproj"
COPY . .
WORKDIR "/src/src/Bootstrapper"
RUN dotnet build "./Bootstrapper.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Bootstrapper.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bootstrapper.dll"]