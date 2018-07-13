using System.Collections.Generic;
using ClickHouse.Net.Entities;

namespace ClickHouse.Net
{
    public interface IClickHouseCommandFormatter
    {
        /// <summary>
        /// Compose a query that performs bulk insert
        /// </summary>
        /// <param name="tableName">table name</param>
        /// <param name="columns">list of column names</param>
        /// <returns></returns>
        string BulkInsert(string tableName, IEnumerable<string> columns);
        
        /// <summary>
        /// Compose a query that creates a table
        /// </summary>
        /// <param name="table">table schema definition</param>
        /// <param name="options">create options</param>
        /// <returns></returns>
        string CreateTable(Table table, CreateOptions options = null);
        
        /// <summary>
        /// Compose a query that creates an empty database
        /// </summary>
        /// <param name="databaseName">database name</param>
        /// <param name="options">create options</param>
        /// <returns></returns>
        string CreateDatabase(string databaseName, CreateOptions options = null);

        /// <summary>
        /// Compose a query that drops a table
        /// </summary>
        /// <param name="name">table name</param>
        /// <param name="options">drop options</param>
        /// <returns></returns>
        string DropTable(string name, DropOptions options = null);
    }
}