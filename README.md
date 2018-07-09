[![Build Status](https://travis-ci.org/ilyabreev/ClickHouse.Net.svg?branch=master)](https://travis-ci.org/ilyabreev/ClickHouse.Net)
[![NuGet version](https://badge.fury.io/nu/ClickHouse.Net.svg)](https://badge.fury.io/nu/ClickHouse.Net)

# ClickHouse.Net
netcore abstractions and helpers for Clickhouse.Ado

## Install

### via Package Manager
```powershell
PM> Install-Package ClickHouse.Net
```

### via dotnet CLI
```bash
> dotnet add package ClickHouse.Net --version 0.2.0
```

## Use

In your `Startup.cs` add to `ConfigureServices`:

```c#
services.AddClickHouse();
```

and define how to resolve `ClickHouseConnectionSettings`:

```c#
services.AddTransient(p => new ClickHouseConnectionSettings(connectionString));
```

Then add `IClickHouseDatabase` as dependency in any of your classes.