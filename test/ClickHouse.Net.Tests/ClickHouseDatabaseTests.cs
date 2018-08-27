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
            if (_target.TableExists("Test"))
            {
                _target.DropTable("Test");
            }

            _target.CreateTable(new Table
            {
                Engine = "MergeTree(date, (date, Id, Number, Cost), 8192)",
                Name = "Test",
                Columns = new List<Column>()
                {
                    new Column("date", "Date"),
                    new Column("Id", "String"),
                    new Column("Number", "Int32"),
                    new Column("Cost", "Float64"),
                    new Column("Name", "String"),
                    new Column("Ushort", "UInt16"),
                    new Column("Uint", "UInt32"),

                }
            });

            var testItem = new TestDataItem3
            {
                Id = Guid.NewGuid(),
                Number = -96,
                Cost = 31,
                Name = "Jon Skeet",
                Ushort = 150,
                Uint = 65536
            };

            var command = testItem.GetInsertCommand();
            _target.ExecuteNonQuery(command);

            command = "SELECT Id, Number, Cost, Name, Ushort, Uint FROM Test";
            var resultItem = _target.ExecuteQueryMapping<TestDataItem3>(command, convention: new UnderscoreNamingConvention()).Single();

            _target.DropTable("Test");
            Assert.Equal(testItem, resultItem);
        }
    }
}
