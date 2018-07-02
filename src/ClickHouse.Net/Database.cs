using System.Collections.Generic;

namespace ClickHouse.Net
{
    public class Database
    {
        public string Name { get; set; }

        public List<Table> Tables { get; set; }
    }
}
