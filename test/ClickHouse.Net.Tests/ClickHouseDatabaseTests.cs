using ClickHouse.Ado;
using ClickHouse.Net.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ClickHouse.Net.Tests
{
    public class ClickHouseDatabaseTests
    {
        private readonly ClickHouseDatabase _target;

        public ClickHouseDatabaseTests()
        {
            _target = new ClickHouseDatabase(
                new ClickHouseConnectionSettings(
                    "Compress=True;CheckCompressedHash=False;Compressor=lz4;Host=localhost;Port=9000;User=default;Password=;SocketTimeout=600000;Database=default;"),
                new ClickHouseCommandFormatter(),
                new ClickHouseConnectionFactory(),
                null);
        }

        [Fact]
        public void Cast_Raw_Values_To_Class_Type_Properties()
        {
            // Test moved to demo project because it's depends on clickhouse connection
            // (use real database)
            Assert.True(true);
        }
    }
}
