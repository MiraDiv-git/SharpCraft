@echo off
if "%~1"=="" (
  echo Error: target OS not specified. Usage: build.bat ^<os^> [config]
  echo Example: build.bat win-x64 Release
  exit /b 1
)

set OS=%~1
set CONFIG=%~2
if "%CONFIG%"=="" set CONFIG=Release

dotnet publish SharpCraft/SharpCraft.csproj ^
  -c %CONFIG% ^
  -p:PublishProfile=%OS%

echo Published to SharpCraft\bin\%CONFIG%\%OS%\