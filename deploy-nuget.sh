ApiKey=$1
Source=$2

nuget pack ./ClickHouse.Net.nuspec -Verbosity detailed
nuget push ./ClickHouse.Net.nupkg -Verbosity detailed -ApiKey $ApiKey -Source $Source