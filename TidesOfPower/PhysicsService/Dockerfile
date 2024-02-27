FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PhysicsService/PhysicsService.csproj", "PhysicsService/"]
COPY ["ClassLibrary/ClassLibrary.csproj", "ClassLibrary/"]
RUN dotnet restore "PhysicsService/PhysicsService.csproj"
COPY . .
WORKDIR "/src/PhysicsService"
RUN dotnet build "PhysicsService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PhysicsService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PhysicsService.dll"]
