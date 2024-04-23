# cd /
# docker build -f RabbitConsumerApp/Dockerfile -t service .
# docker run -p 8082:80 service

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["RabbitConsumerApp/RabbitConsumerApp.csproj", "RabbitConsumerApp/"]
COPY ["ClassLibrary/ClassLibrary.csproj", "ClassLibrary/"]
RUN dotnet restore "RabbitConsumerApp/RabbitConsumerApp.csproj"
COPY . .
WORKDIR "/src/RabbitConsumerApp"
RUN dotnet build "RabbitConsumerApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RabbitConsumerApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RabbitConsumerApp.dll"]
