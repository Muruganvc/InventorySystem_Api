# See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# 👇 Go up one level from current context to copy projects
COPY ../InventorySystem_Api/InventorySystem_Api.csproj ./InventorySystem_Api/
COPY ../InventorySystem_Application/InventorySystem_Application.csproj ./InventorySystem_Application/
COPY ../InventorySystem_Domain/InventorySystem_Domain.csproj ./InventorySystem_Domain/
COPY ../InventorySystem_Infrastructure/InventorySystem_Infrastructure.csproj ./InventorySystem_Infrastructure/

RUN dotnet restore ./InventorySystem_Api/InventorySystem_Api.csproj

# 👇 Copy everything from solution root
COPY .. .

WORKDIR /src/InventorySystem_Api
RUN dotnet build "InventorySystem_Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InventorySystem_Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InventorySystem_Api.dll"]
