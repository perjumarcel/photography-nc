# syntax=docker/dockerfile:1.7

# ─── Build stage ─────────────────────────────────────────────────────────────
# Pinned to .NET 10 preview SDK (matches global.json).
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy only the project files first to maximise Docker layer caching for
# `dotnet restore`. Subsequent source changes won't bust the restore cache.
COPY Photography.slnx ./
COPY src/Photography.SharedKernel/*.csproj ./src/Photography.SharedKernel/
COPY src/Photography.Core/*.csproj          ./src/Photography.Core/
COPY src/Photography.Application/*.csproj   ./src/Photography.Application/
COPY src/Photography.Infrastructure/*.csproj ./src/Photography.Infrastructure/
COPY src/Photography.Web/*.csproj           ./src/Photography.Web/
COPY tools/Photography.Migrator/*.csproj    ./tools/Photography.Migrator/
COPY tests/Photography.Application.Tests/*.csproj ./tests/Photography.Application.Tests/
COPY tests/Photography.Infrastructure.Tests/*.csproj ./tests/Photography.Infrastructure.Tests/
COPY global.json ./

RUN dotnet restore src/Photography.Web/Photography.Web.csproj

COPY . .
RUN dotnet publish src/Photography.Web/Photography.Web.csproj \
    --configuration ${BUILD_CONFIGURATION} \
    --no-restore \
    -o /app/publish \
    /p:UseAppHost=false

# ─── Runtime stage ───────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Bind to all interfaces inside the container; the host maps the port.
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_RUNNING_IN_CONTAINER=true

EXPOSE 8080

# Healthchecks are intentionally left to the orchestrator (k8s/ECS/compose),
# which can probe the richer `/health/ready` endpoint over HTTP without
# needing a shell or curl inside the image.

ENTRYPOINT ["dotnet", "Photography.Web.dll"]
