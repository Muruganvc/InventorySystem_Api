# Use ASP.NET 7 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use .NET SDK for build
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy only the .csproj file
COPY ["InventorySystem_Api.csproj", "."]
RUN dotnet restore "InventorySystem_Api.csproj"

# Copy all files for build
COPY . .
RUN dotnet build "InventorySystem_Api.csproj" -c Release -o /app/build

# Publish the app
FROM build AS publish
RUN dotnet publish "InventorySystem_Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InventorySystem_Api.dll"]
