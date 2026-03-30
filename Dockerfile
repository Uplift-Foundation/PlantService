FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["PlantService/PlantService.csproj", "PlantService/"]
COPY ["FMN.Vault/FMN.Vault.csproj", "FMN.Vault/"]
RUN dotnet restore "PlantService/PlantService.csproj"
COPY PlantService/ PlantService/
COPY FMN.Vault/ FMN.Vault/
WORKDIR "/src/PlantService"
RUN dotnet build "PlantService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PlantService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PlantService.dll"]
