FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8003

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["StatisticsService.csproj", "./"]
RUN dotnet restore "StatisticsService.csproj"
COPY . .
RUN dotnet build "StatisticsService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StatisticsService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8003
ENTRYPOINT ["dotnet", "StatisticsService.dll"]

