#!/usr/bin/env bash
# Use the following
# ~ ./deploy.sh windows|mojave|linux
PLATFORM=$1

echo "Platform: $PLATFORM";

case $PLATFORM in
    "windows" ) RUNTIME="win-x64"
    ;;
    "mojave" ) RUNTIME="osx.10.14-x64"
    ;;
    "linux" ) RUNTIME="linux-x64"
    ;;
esac

echo "Runtime: $RUNTIME";

dotnet publish SolStandard.csproj --self-contained --runtime $RUNTIME --configuration Release;