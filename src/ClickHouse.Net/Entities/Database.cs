using System.Collections.Generic;

namespace ClickHouse.Net.Entities
{
    /// <summary>
    /// Represents a ClickHouse database
    /// </summary>
    public class Database
    {
        /// <summary>
        /// Database name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// List of database tables
        /// </summary>
        public List<Table> Tables { get; set; }
    }
}
