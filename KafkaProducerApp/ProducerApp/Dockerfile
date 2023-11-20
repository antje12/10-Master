FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["KafkaProducerApp/KafkaProducerApp.csproj", "KafkaProducerApp/"]
RUN dotnet restore "KafkaProducerApp/KafkaProducerApp.csproj"
COPY . .
WORKDIR "/src/KafkaProducerApp"
RUN dotnet build "KafkaProducerApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KafkaProducerApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KafkaProducerApp.dll"]
