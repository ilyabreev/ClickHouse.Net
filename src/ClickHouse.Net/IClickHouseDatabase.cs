using System.Collections.Generic;
using System.Data;
using ClickHouse.Ado;
using ClickHouse.Net.Entities;

namespace ClickHouse.Net
{
    /// <summary>
    /// Интерфес управления структурой базы данных
    /// </summary>
    public interface IClickHouseDatabase
    {
        /// <summary>
        /// Create a new database
        /// </summary>
        /// <param name="db">Object representing database structure</param>
        void CreateDatabase(Database db, CreateOptions options = null);

        /// <summary>
        /// Create a new empty database
        /// </summary>
        /// <param name="databaseName">database name</param>
        /// <param name="options">create options</param>
        void CreateDatabase(string databaseName, CreateOptions options = null);
        
        /// <summary>
        /// Проверяет существование базы данных
        /// </summary>
        /// <param name="databaseName"></param>
        bool DatabaseExists(string databaseName);
        
        /// <summary>
        /// Check if table exists in current database
        /// </summary>
        /// <param name="tableName">Table name</param>
        bool TableExists(string tableName);

        /// <summary>
        /// Drop a table
        /// </summary>
        /// <param name="tableName">table name</param>
        /// <param name="options">drop options</param>
        void DropTable(string tableName, DropOptions options = null);
        
        /// <summary>
        /// Переименовать таблицы
        /// </summary>
        /// <param name="currentName">Текущее название</param>
        /// <param name="newName">Новое название</param>
        void RenameTable(string currentName, string newName);
        
        /// <summary>
        /// Выполнить запрос, который возвращает число 
        /// </summary>
        /// <param name="command">запрос</param>
        /// <returns></returns>
        bool ExecuteExists(IDbCommand command);

        /// <summary>
        /// Выборка строк из БД
        /// </summary>
        /// <param name="commandText">запрос</param>
        /// <returns></returns>
        object[][] ExecuteSelectCommand(string commandText, bool header = false);


        IEnumerable<string> ReadAsStringsList(string commandText);

        /// <summary>
        /// Change current connection settings for database. It doesn't affect already opened connection.
        /// </summary>
        /// <param name="connectionSettings"></param>
        void ChangeConnectionSettings(ClickHouseConnectionSettings connectionSettings);

        /// <summary>
        /// Open a database connection in order to execute multiple commands inside one connection
        /// </summary>
        void Open();

        /// <summary>
        /// Close a current database connection
        /// </summary>
        void Close();

        /// <summary>
        /// Change current database if there is owned connection opened
        /// </summary>
        /// <param name="dbName"></param>
        void ChangeDatabase(string dbName);

        /// <summary>
        /// Execute a query that produces no results
        /// </summary>
        /// <param name="commandText">Text of query</param>
        void ExecuteNonQuery(string commandText);
        
        /// <summary>
        /// Bulk insert rows into table
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="tableName">Table name</param>
        /// <param name="columns">List of columns</param>
        /// <param name="bulk">Data to insert</param>
        void BulkInsert<T>(string tableName, IEnumerable<string> columns, IEnumerable<T> bulk);

        /// <summary>
        /// Create a new table
        /// </summary>
        /// <param name="table">table schema</param>
        /// <param name="options">create options</param>
        void CreateTable(Table table, CreateOptions options = null);

        /// <summary>
        /// Execute a query that maps its result to object collection
        /// </summary>
        /// <typeparam name="T">object type</typeparam>
        /// <param name="commandText">query text</param>
        /// <param name="parameters">list of parameters</param>
        /// <param name="convention">column naming convention</param>
        /// <returns></returns>
        IEnumerable<T> ExecuteQueryMapping<T>(string commandText, IEnumerable<ClickHouseParameter> parameters = null, IColumnNamingConvention convention = null) where T : new();

        /// <summary>
        /// Get list of database partitions
        /// </summary>
        /// <param name="database">database name</param>
        /// <returns></returns>
        Part[] SystemParts(string database);

        /// <summary>
        /// Freeze a single partition of a table
        /// </summary>
        /// <param name="table">table name</param>
        /// <param name="partition">partition</param>
        void FreezePartition(string table, string partition);

        /// <summary>
        /// Backup a database
        /// </summary>
        /// <param name="database">database name</param>
        void BackupDatabase(string database);

        /// <summary>
        /// Get list of table columns
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        IEnumerable<Column> DescribeTable(string tableName);
    }
}