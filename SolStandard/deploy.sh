# Use the following
# ~ ./deploy.sh WINDOWS10|MOJAVE|UBUNTU|REDHAT
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