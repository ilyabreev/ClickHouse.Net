using System.Collections.Generic;

namespace ClickHouse.Net
{
    public class ClickHouseCommandFormatter : IClickHouseCommandFormatter
    {        
        public string CreateInsertCommandText(string tableName, IEnumerable<string> columns)
        {
            return $@"INSERT INTO {tableName}({string.Join(',', columns)}) VALUES @bulk";
        }
    }
}
