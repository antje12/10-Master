FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ProjectileService/ProjectileService.csproj", "ProjectileService/"]
COPY ["ClassLibrary/ClassLibrary.csproj", "ClassLibrary/"]
RUN dotnet restore "ProjectileService/ProjectileService.csproj"
COPY . .
WORKDIR "/src/ProjectileService"
RUN dotnet build "ProjectileService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProjectileService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProjectileService.dll"]
