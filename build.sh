#!/bin/bash

if [ -z "$1" ]; then
  echo "Error: target OS not specified. Usage: ./publish.sh <os> [config]"
  echo "Example: ./publish.sh win-x64 Release"
  exit 1
fi

OS=$1
CONFIG=${2:-Debug}

dotnet publish SharpCraft/SharpCraft.csproj \
  -c $CONFIG \
  -r $OS \
  --self-contained true \
  -p:PublishSingleFile=true \
  -o ./publish/$OS/$CONFIG
