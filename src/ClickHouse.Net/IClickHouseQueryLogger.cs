namespace ClickHouse.Net
{
    public interface IClickHouseQueryLogger
    {
        void BeforeQuery();
        void AfterQuery(string queryText);
    }
}
