﻿FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["WorldService/WorldService.csproj", "WorldService/"]
COPY ["ClassLibrary/ClassLibrary.csproj", "ClassLibrary/"]
RUN dotnet restore "WorldService/WorldService.csproj"
COPY . .
WORKDIR "/src/WorldService"
RUN dotnet build "WorldService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WorldService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WorldService.dll"]
