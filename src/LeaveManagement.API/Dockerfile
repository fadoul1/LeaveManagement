# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /repo

# Copy solution and project files
COPY LeaveManagement.slnx .
COPY src/LeaveManagement.Domain/LeaveManagement.Domain.csproj src/LeaveManagement.Domain/
COPY src/LeaveManagement.Application/LeaveManagement.Application.csproj src/LeaveManagement.Application/
COPY src/LeaveManagement.Persistence/LeaveManagement.Persistence.csproj src/LeaveManagement.Persistence/
COPY src/LeaveManagement.ExternalServices/LeaveManagement.ExternalServices.csproj src/LeaveManagement.ExternalServices/
COPY _aspire/LeaveManagement.ServiceDefaults/LeaveManagement.ServiceDefaults.csproj _aspire/LeaveManagement.ServiceDefaults/
COPY src/LeaveManagement.API/LeaveManagement.API.csproj src/LeaveManagement.API/

# Restore dependencies
RUN dotnet restore src/LeaveManagement.API/LeaveManagement.API.csproj

# Copy source code
COPY src/LeaveManagement.Domain/ src/LeaveManagement.Domain/
COPY src/LeaveManagement.Application/ src/LeaveManagement.Application/
COPY src/LeaveManagement.Persistence/ src/LeaveManagement.Persistence/
COPY src/LeaveManagement.ExternalServices/ src/LeaveManagement.ExternalServices/
COPY _aspire/LeaveManagement.ServiceDefaults/ _aspire/LeaveManagement.ServiceDefaults/
COPY src/LeaveManagement.API/ src/LeaveManagement.API/

# Build and publish
WORKDIR /repo/src/LeaveManagement.API
RUN dotnet publish -c Release -o /app/publish --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Use the built-in non-root user provided by .NET 8+ runtime images
USER app

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "LeaveManagement.API.dll"]
