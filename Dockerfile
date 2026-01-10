# Multi-stage Dockerfile for DATERP (ABP Framework)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy all projects to preserve solution structure
COPY . .

# Restore dependencies
RUN dotnet restore DATERP.sln

# Build and Publish the Web project
WORKDIR /src/src/DATERP.Web
RUN dotnet publish -c Release -o /app/publish

# Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 80
ENTRYPOINT ["dotnet", "DATERP.Web.dll"]
