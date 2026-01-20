# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy solution and project files
COPY .  . 

# Restore dependencies
RUN dotnet restore

# Build the application
RUN dotnet build -c Release --no-restore

# Publish the application
RUN dotnet publish -c Release -o /app/publish --no-build

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copy published application from build stage
COPY --from=build /app/publish .  

# Expose port 8076 for the API
EXPOSE 8076

# Set environment variables for User Secrets
ENV ASPNETCORE_URLS=http://+:8076

# Run the application
ENTRYPOINT ["dotnet", "HR.MobyPark. dll"]
