# Dockerfile para ASP.NET Core + EF Core + PostgreSQL/SQLite en Render y local
# Etapa base: solo runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

# Etapa build: compila y publica la app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY DarkCloud.csproj ./
RUN dotnet restore "DarkCloud.csproj"
COPY . .
RUN dotnet build "DarkCloud.csproj" -c Release -o /app/build
RUN dotnet publish "DarkCloud.csproj" -c Release -o /app/publish

# Etapa final: solo archivos necesarios para producción
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
# Copiar explícitamente la carpeta de migraciones para EF Core
COPY --from=build /src/Migrations ./Migrations
COPY entrypoint.sh ./entrypoint.sh
RUN chmod +x ./entrypoint.sh
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["./entrypoint.sh"]
