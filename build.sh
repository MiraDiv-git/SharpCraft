#!/bin/bash

set -e

if [ -z "$1" ]; then
  echo "Error: target OS not specified. Usage: ./build.sh <os> [config]"
  echo "Example: ./build.sh linux-x64 Release"
  exit 1
fi

OS=$1
CONFIG=${2:-Release}

dotnet publish SharpCraft/SharpCraft.csproj \
  -c "$CONFIG" \
  -p:PublishProfile="$OS"

echo "Published to SharpCraft/bin/$CONFIG/$OS/"