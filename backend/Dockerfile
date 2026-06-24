# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /repo

# Copy root build configuration (required for Central Package Management)
COPY Directory.Build.props .
COPY Directory.Packages.props .
COPY global.json .

# Copy solution and project files for restore layer cache
COPY backend/LeaveManagement.slnx backend/
COPY backend/src/LeaveManagement.Domain/LeaveManagement.Domain.csproj backend/src/LeaveManagement.Domain/
COPY backend/src/LeaveManagement.Application/LeaveManagement.Application.csproj backend/src/LeaveManagement.Application/
COPY backend/src/LeaveManagement.Persistence/LeaveManagement.Persistence.csproj backend/src/LeaveManagement.Persistence/
COPY backend/src/LeaveManagement.ExternalServices/LeaveManagement.ExternalServices.csproj backend/src/LeaveManagement.ExternalServices/
COPY _aspire/LeaveManagement.ServiceDefaults/LeaveManagement.ServiceDefaults.csproj _aspire/LeaveManagement.ServiceDefaults/
COPY backend/src/LeaveManagement.API/LeaveManagement.API.csproj backend/src/LeaveManagement.API/

# Restore dependencies
RUN dotnet restore backend/src/LeaveManagement.API/LeaveManagement.API.csproj

# Copy source code
COPY backend/src/LeaveManagement.Domain/ backend/src/LeaveManagement.Domain/
COPY backend/src/LeaveManagement.Application/ backend/src/LeaveManagement.Application/
COPY backend/src/LeaveManagement.Persistence/ backend/src/LeaveManagement.Persistence/
COPY backend/src/LeaveManagement.ExternalServices/ backend/src/LeaveManagement.ExternalServices/
COPY _aspire/LeaveManagement.ServiceDefaults/ _aspire/LeaveManagement.ServiceDefaults/
COPY backend/src/LeaveManagement.API/ backend/src/LeaveManagement.API/

# Build and publish
WORKDIR /repo/backend/src/LeaveManagement.API
RUN dotnet publish -c Release -o /app/publish --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

USER app

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "LeaveManagement.API.dll"]
