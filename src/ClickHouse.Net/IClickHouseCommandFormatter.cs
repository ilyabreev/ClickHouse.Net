using System.Collections.Generic;

namespace ClickHouse.Net
{
    public interface IClickHouseCommandFormatter
    {
        /// <summary>
        /// Создать команду для вставки данных
        /// </summary>
        /// <param name="tableName">Имя таблицы, в которую необходимо выполнить вставку</param>
        /// <param name="columns">Имена столбцов, в которые необходимо вставить данные</param>
        /// <returns></returns>
        string CreateInsertCommandText(string tableName, IEnumerable<string> columns);

        string CreateDatabase(string dbName, bool ifNotExists = true);
        string Create(string subject, string rest, bool ifNotExists = true);
    }
}