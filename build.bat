@echo off
if "%~1"=="" (
  echo Error: target OS not specified. Usage: build.bat ^<os^> [config]
  echo Example: build.bat win-x64 Release
  exit /b 1
)

set OS=%~1
set CONFIG=%~2
if "%CONFIG%"=="" set CONFIG=Debug

dotnet publish SharpCraft/SharpCraft.csproj ^
  -c %CONFIG% ^
  -r %OS% ^
  --self-contained true ^
  -p:PublishSingleFile=true ^
  -o ./publish/%CONFIG%/%OS%
