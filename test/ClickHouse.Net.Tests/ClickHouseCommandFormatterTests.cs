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
    }
}
