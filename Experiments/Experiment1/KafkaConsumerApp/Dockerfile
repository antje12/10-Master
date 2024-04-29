# cd /
# docker build -f KafkaConsumerApp/Dockerfile -t service .
# docker run -p 8082:80 service

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["KafkaConsumerApp/KafkaConsumerApp.csproj", "KafkaConsumerApp/"]
COPY ["ClassLibrary/ClassLibrary.csproj", "ClassLibrary/"]
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
