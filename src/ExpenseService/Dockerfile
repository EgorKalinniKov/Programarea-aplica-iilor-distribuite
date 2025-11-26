FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ExpenseService.csproj", "./"]
RUN dotnet restore "ExpenseService.csproj"
COPY . .
RUN dotnet build "ExpenseService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ExpenseService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8001
ENTRYPOINT ["dotnet", "ExpenseService.dll"]

