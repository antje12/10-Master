# cd /
# docker build -f InputService/Dockerfile -t service .
# docker run -p 81:80 service

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["InputService/InputService.csproj", "InputService/"]
COPY ["ClassLibrary/ClassLibrary.csproj", "ClassLibrary/"]
RUN dotnet restore "InputService/InputService.csproj"
COPY . .
WORKDIR "/src/InputService"
RUN dotnet build "InputService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InputService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InputService.dll"]
