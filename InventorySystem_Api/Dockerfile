# Use ASP.NET runtime as base
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use .NET SDK to build
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy project files (use relative paths since root is InventorySystem_Api)
COPY InventorySystem_Api.csproj ./InventorySystem_Api/
COPY ../InventorySystem_Application/InventorySystem_Application.csproj ./InventorySystem_Application/
COPY ../InventorySystem_Domain/InventorySystem_Domain.csproj ./InventorySystem_Domain/
COPY ../InventorySystem_Infrastructure/InventorySystem_Infrastructure.csproj ./InventorySystem_Infrastructure/

# Restore dependencies
RUN dotnet restore ./InventorySystem_Api/InventorySystem_Api.csproj

# Copy the rest of the code
COPY . ./InventorySystem_Api/
COPY ../InventorySystem_Application ./InventorySystem_Application/
COPY ../InventorySystem_Domain ./InventorySystem_Domain/
COPY ../InventorySystem_Infrastructure ./InventorySystem_Infrastructure/

# Build the project
WORKDIR /src/InventorySystem_Api
RUN dotnet build InventorySystem_Api.csproj -c Release -o /app/build

# Publish the project
FROM build AS publish
RUN dotnet publish InventorySystem_Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InventorySystem_Api.dll"]
