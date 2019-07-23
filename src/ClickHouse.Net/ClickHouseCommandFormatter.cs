using System;
using System.Collections.Generic;
using System.Linq;
using ClickHouse.Net.Entities;

namespace ClickHouse.Net
{
    public class ClickHouseCommandFormatter : IClickHouseCommandFormatter
    {        
        public string BulkInsert(string tableName, IEnumerable<string> columns)
        {
            return $@"INSERT INTO {tableName}({string.Join(",", columns)}) VALUES @bulk";
        }

        public string CreateTable(Table table, CreateOptions options = null)
        {           
            return Create("TABLE", Table(table), options);
        }

        public string DropTable(string name, DropOptions options = null)
        {
            return Drop("TABLE", name, options);
        }
        
        public string CreateDatabase(string databaseName, CreateOptions options = null)
        {
            return Create("DATABASE", databaseName, options);
        }

        public string DescribeTable(string name)
        {
            return $"DESCRIBE TABLE {name}";
        }

        private string Table(Table table)
        {
            return $"{table.Name}{TableSchema(table)} ENGINE = {table.Engine}{TableSelect(table)}";
        }

        private string TableSchema(Table table)
        {
            return table.Columns != null && table.Columns.Any() ? $" {Columns(table.Columns)}" : string.Empty;
        }

        private string TableSelect(Table table)
        {
            return string.IsNullOrWhiteSpace(table.Select) ? String.Empty : $" AS {table.Select}";
        }
        
        private string Columns(IEnumerable<Column> columns)
        {
            return $"({string.Join(", ", columns)})";
        }

        private string Create(string subject, string rest, CreateOptions options = null)
        {
            return $"CREATE {subject}{IfNotExists(options)} {rest}";
        }

        private string Drop(string subject, string name, DropOptions options = null)
        {
            return $"DROP {subject}{IfExists(options)} {name}";
        }

        private string IfNotExists(CreateOptions options = null)
        {
            var ifNotExists = options?.IfNotExists ?? true;
            return ifNotExists ? " IF NOT EXISTS" : "";
        }

        private string IfExists(DropOptions options = null)
        {
            var ifExists = options?.IfExists ?? true;
            return ifExists ? " IF EXISTS" : "";
        }
    }
}
