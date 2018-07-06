using ClickHouse.Ado;

namespace ClickHouse.Net
{
    public interface IClickHouseConnectionFactory
    {
        ClickHouseConnection CreateConnection(ClickHouseConnectionSettings connectionSettings);
    }
}