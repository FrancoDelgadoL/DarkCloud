#!/bin/sh
# Script de entrada para aplicar migraciones y luego iniciar la app
set -e

# Aplicar migraciones (ignora error si ya están aplicadas)
dotnet ef database update --no-build || true

# Iniciar la aplicación
exec dotnet DarkCloud.dll
