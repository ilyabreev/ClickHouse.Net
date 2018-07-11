using System.Collections.Generic;

namespace ClickHouse.Net
{
    public class ClickHouseCommandFormatter : IClickHouseCommandFormatter
    {        
        public string CreateInsertCommandText(string tableName, IEnumerable<string> columns)
        {
            return $@"INSERT INTO {tableName}({string.Join(",", columns)}) VALUES @bulk";
        }

        public string Create(string subject, string rest, bool ifNotExists = true)
        {
            return $"CREATE {subject}{(ifNotExists ? " IF NOT EXISTS" : "")} {rest}";
        }

        public string CreateDatabase(string dbName, bool ifNotExists = true)
        {
            return Create("DATABASE", dbName, ifNotExists);
        }
    }
}
