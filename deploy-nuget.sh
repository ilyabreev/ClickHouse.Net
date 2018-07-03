ApiKey=$1
Source=$2

dotnet pack -c Release --no-restore --no-build -o ../../ ./src/ClickHouse.Net/ClickHouse.Net.csproj 
dotnet nuget push -k $ApiKey -s $Source ./ClickHouse.Net.*.nupkg 