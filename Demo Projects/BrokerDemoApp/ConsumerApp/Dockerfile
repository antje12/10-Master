# cd /
# docker build -f ConsumerApp/Dockerfile -t service .
# docker run -p 8082:80 service

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ConsumerApp/ConsumerApp.csproj", "ConsumerApp/"]
COPY ["ClassLibrary/ClassLibrary.csproj", "ClassLibrary/"]
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
