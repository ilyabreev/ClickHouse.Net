ApiKey=$1
Source=$2

dotnet pack -c Release --no-restore --no-build ./src/ClickHouse.Net/ClickHouse.Net.csproj -o ../../ -v detailed
dotnet nuget push ./ClickHouse.Net.*.nupkg -v detailed -k $ApiKey -s $Source