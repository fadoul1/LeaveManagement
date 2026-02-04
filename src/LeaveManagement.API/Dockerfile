# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY LeaveManagement.sln .
COPY LeaveManagement.Domain/LeaveManagement.Domain.csproj LeaveManagement.Domain/
COPY LeaveManagement.Application/LeaveManagement.Application.csproj LeaveManagement.Application/
COPY LeaveManagement.Persistence/LeaveManagement.Persistence.csproj LeaveManagement.Persistence/
COPY LeaveManagement.ExternalServices/LeaveManagement.ExternalServices.csproj LeaveManagement.ExternalServices/
COPY LeaveManagement.ServiceDefaults/LeaveManagement.ServiceDefaults.csproj LeaveManagement.ServiceDefaults/
COPY LeaveManagement.API/LeaveManagement.API.csproj LeaveManagement.API/

# Restore dependencies
RUN dotnet restore LeaveManagement.API/LeaveManagement.API.csproj

# Copy source code
COPY LeaveManagement.Domain/ LeaveManagement.Domain/
COPY LeaveManagement.Application/ LeaveManagement.Application/
COPY LeaveManagement.Persistence/ LeaveManagement.Persistence/
COPY LeaveManagement.ExternalServices/ LeaveManagement.ExternalServices/
COPY LeaveManagement.ServiceDefaults/ LeaveManagement.ServiceDefaults/
COPY LeaveManagement.API/ LeaveManagement.API/

# Build and publish
WORKDIR /src/LeaveManagement.API
RUN dotnet publish -c Release -o /app/publish --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Create non-root user for security
RUN adduser --disabled-password --gecos "" appuser
USER appuser

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "LeaveManagement.API.dll"]
