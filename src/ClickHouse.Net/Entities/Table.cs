using System.Collections.Generic;

namespace ClickHouse.Net.Entities
{
    /// <summary>
    /// Representing a table in ClickHouse database
    /// </summary>
    public class Table
    {
        /// <summary>
        /// Table name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// List of table columns
        /// </summary>
        public List<Column> Columns { get; set; }

        /// <summary>
        /// Table engine
        /// </summary>
        public string Engine { get; set; }
    }
}
