FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8002

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["CategoryService.csproj", "./"]
RUN dotnet restore "CategoryService.csproj"
COPY . .
RUN dotnet build "CategoryService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CategoryService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8002
ENTRYPOINT ["dotnet", "CategoryService.dll"]

