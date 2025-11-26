FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8004

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ImportExportService.csproj", "./"]
RUN dotnet restore "ImportExportService.csproj"
COPY . .
RUN dotnet build "ImportExportService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ImportExportService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8004
ENTRYPOINT ["dotnet", "ImportExportService.dll"]

