using System.Collections.Generic;

namespace ClickHouse.Net.Entities
{
    /// <summary>
    /// Representing a table schema in ClickHouse database
    /// </summary>
    public class Table
    {
        /// <summary>
        /// Table name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// List of table column schemas
        /// </summary>
        public List<Column> Columns { get; set; }

        /// <summary>
        /// SELECT query that represents table
        /// </summary>
        public string Select { get; set; }

        /// <summary>
        /// Table engine
        /// </summary>
        public string Engine { get; set; }
    }
}
