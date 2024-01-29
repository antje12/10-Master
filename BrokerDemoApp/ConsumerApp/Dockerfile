FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ConsumerApp/ConsumerApp.csproj", "ConsumerApp/"]
RUN dotnet restore "ConsumerApp/ConsumerApp.csproj"
COPY . .
WORKDIR "/src/ConsumerApp"
RUN dotnet build "ConsumerApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ConsumerApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ConsumerApp.dll"]
