#!/bin/bash

# First parameter is build mode, defaults to Debug

MODE=${1:-Debug}
NAME="KickOff"

dotnet restore

dotnet build ./src/$NAME --configuration $MODE
dotnet build ./src/$NAME.Tests --configuration $MODE

dotnet test ./src/$NAME.Tests --configuration $MODE

mkdir -p ./build/deploy
dotnet pack ./src/$NAME --configuration $MODE --output ./build/deploy
