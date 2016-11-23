#!/bin/bash

# First parameter is build mode, defaults to Debug

MODE=${1:-Debug}

mkdir -p ./build/deploy

dotnet restore

dotnet build **/project.json --configuration $MODE
dotnet test **/*.Tests --configuration $MODE
dotnet pack ./src/KickOff --configuration $MODE --output ./build/deploy
dotnet pack ./src/KickOff.Host.Windows --configuration $MODE --output ./build/deploy
