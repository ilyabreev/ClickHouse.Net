using Microsoft.Extensions.DependencyInjection;

namespace ClickHouse.Net
{
    /// <summary>
    /// Extensions for registering clickhouse in ASP.NET Core DI
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static void AddClickHouse(this IServiceCollection services)
        {
            services.AddTransient<IClickHouseConnectionFactory, ClickHouseConnectionFactory>();
            services.AddTransient<IClickHouseDatabase, ClickHouseDatabase>();
            services.AddTransient<IClickHouseQueryLogger, ClickHouseQueryLogger>();
            services.AddTransient<IClickHouseCommandFormatter, ClickHouseCommandFormatter>();
            services.AddTransient<IClickHouseDataFormatter, ClickHouseDataFormatter>();
        }
    }
}
