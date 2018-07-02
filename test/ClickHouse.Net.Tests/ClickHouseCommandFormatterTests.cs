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
            // arrange 
            var tableName = "testTable";
            var columns = new string[] { "column1", "column2", "column3" };
            var expectedCommandText = "INSERT INTO testTable(column1,column2,column3) VALUES @bulk";

            // act
            var result = _target.CreateInsertCommandText(tableName, columns);

            // assert
            Assert.Equal(expectedCommandText, result);
        }
    }
}
