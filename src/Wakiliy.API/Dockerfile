# 1. Base stage: Uses the lightweight ASP.NET Core runtime image
# This stage is used as the foundation for running the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# 2. Build stage: Uses the full .NET SDK to compile the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the project files and restore the NuGet packages
COPY ["Wakiliy.API/Wakiliy.API.csproj", "Wakiliy.API/"]
COPY ["Wakiliy.Application/Wakiliy.Application.csproj", "Wakiliy.Application/"]
COPY ["Wakiliy.Domain/Wakiliy.Domain.csproj", "Wakiliy.Domain/"]
COPY ["Wakiliy.Infrastructure/Wakiliy.Infrastructure.csproj", "Wakiliy.Infrastructure/"]
RUN dotnet restore "./Wakiliy.API/Wakiliy.API.csproj"

# Copy the remaining source code and build the application
COPY . .
WORKDIR "/src/Wakiliy.API"
RUN dotnet build "./Wakiliy.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# 3. Publish stage: Publishes the application to a folder for deployment
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Wakiliy.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# 4. Final stage: Copies the published app from the publish stage into the base image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Wakiliy.API.dll"]
