#!/bin/bash

if [ -z "$1" ]; then
  echo "Error: target OS not specified. Usage: ./build.sh <os> [config]"
  echo "Example: ./build.sh linux-x64 Release"
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
