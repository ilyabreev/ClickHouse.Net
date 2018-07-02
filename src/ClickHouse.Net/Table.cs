using System.Collections.Generic;

namespace ClickHouse.Net
{
    public class Table
    {
        public string Name { get; set; }

        public List<Column> Columns { get; set; }

        public string Engine { get; set; }
    }
}
