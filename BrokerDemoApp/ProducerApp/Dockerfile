FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ProducerApp/ProducerApp.csproj", "ProducerApp/"]
RUN dotnet restore "ProducerApp/ProducerApp.csproj"
COPY . .
WORKDIR "/src/ProducerApp"
RUN dotnet build "ProducerApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProducerApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProducerApp.dll"]
