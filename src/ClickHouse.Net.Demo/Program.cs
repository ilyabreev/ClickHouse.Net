using System;
using ClickHouse.Ado;

namespace ClickHouse.Net.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new ClickHouseDatabase(
                new ClickHouseConnectionSettings("Compress=True;CheckCompressedHash=False;Compressor=lz4;Host=localhost;Port=9000;User=default;Password=;SocketTimeout=600000;Database=default;"),
                new ClickHouseCommandFormatter(),
                new ClickHouseConnectionFactory(),
                null);
            db.Open();
            for (int i = 0; i < 10; i++)
            {
                var res = db.DatabaseExists("test");
                Console.WriteLine($"Database test exists: {res}");
            }

            db.Close();
        }
    }
}
