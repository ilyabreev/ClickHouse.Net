using ClickHouse.Ado;

namespace ClickHouse.Net
{
    public class ClickHouseConnectionFactory : IClickHouseConnectionFactory
    {
        public ClickHouseConnection CreateConnection(ClickHouseConnectionSettings connectionSettings)
        {
            return new ClickHouseConnection(connectionSettings);
        }
    }
}
