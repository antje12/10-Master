FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["KafkaConsumerApp/KafkaConsumerApp.csproj", "KafkaConsumerApp/"]
RUN dotnet restore "KafkaConsumerApp/KafkaConsumerApp.csproj"
COPY . .
WORKDIR "/src/KafkaConsumerApp"
RUN dotnet build "KafkaConsumerApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KafkaConsumerApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KafkaConsumerApp.dll"]
