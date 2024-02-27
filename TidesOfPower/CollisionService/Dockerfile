FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["CollisionService/CollisionService.csproj", "CollisionService/"]
COPY ["ClassLibrary/ClassLibrary.csproj", "ClassLibrary/"]
RUN dotnet restore "CollisionService/CollisionService.csproj"
COPY . .
WORKDIR "/src/CollisionService"
RUN dotnet build "CollisionService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CollisionService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CollisionService.dll"]
