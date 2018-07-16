using System.Collections.Generic;
using ClickHouse.Net.Entities;
using Xunit;

namespace ClickHouse.Net.Tests
{
    public class ClickHouseCommandFormatterTests
    {
        private readonly ClickHouseCommandFormatter _target;

        public ClickHouseCommandFormatterTests()
        {
            _target = new ClickHouseCommandFormatter();
        }

        [Fact]
        public void Create_Insert_Command_Text()
        {
            var tableName = "testTable";
            var columns = new string[] { "column1", "column2", "column3" };
            var expectedCommandText = "INSERT INTO testTable(column1,column2,column3) VALUES @bulk";

            var result = _target.BulkInsert(tableName, columns);

            Assert.Equal(expectedCommandText, result);
        }

        [Fact]
        public void CreateDatabase_IfNotExists_Command_Text()
        {
            Assert.Equal("CREATE DATABASE IF NOT EXISTS Test", _target.CreateDatabase("Test"));
        }

        [Fact]
        public void CreateDatabase_Command_Text()
        {
            Assert.Equal("CREATE DATABASE Test", _target.CreateDatabase("Test", new CreateOptions()
            {
                IfNotExists = false
            }));
        }

        [Fact]
        public void DropTable_IfExists_Command_Text()
        {
            Assert.Equal("DROP TABLE IF EXISTS Test", _target.DropTable("Test"));
        }

        [Fact]
        public void DropTable_Command_Text()
        {
            Assert.Equal("DROP TABLE Test", _target.DropTable("Test", new DropOptions()
            {
                IfExists = false
            }));
        }

        [Fact]
        public void CreateTable_Columns_Command_Text()
        {
            Assert.Equal("CREATE TABLE IF NOT EXISTS Test (date Date, Col1 UInt8, Col2 String) ENGINE = MergeTree(date, (date, Col1, Col2), 8192)",
                _target.CreateTable(new Table()
                {
                    Engine = "MergeTree(date, (date, Col1, Col2), 8192)",
                    Name = "Test",
                    Columns = new List<Column>()
                    {
                        new Column("date", "Date"),
                        new Column("Col1", "UInt8"),
                        new Column("Col2", "String")
                    }
                }));

        }
    }
}
