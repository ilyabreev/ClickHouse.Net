using ClickHouse.Ado;
using ClickHouse.Net.Entities;
using ClickHouse.Net.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ClickHouse.Net.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString =
                "Compress=True;CheckCompressedHash=False;Compressor=lz4;Host=localhost;Port=9000;User=default;Password=;SocketTimeout=600000;Database=default;";
            var serviceCollection = new ServiceCollection();
            
            /*Default PropertyBinder will be used*/
            serviceCollection.AddClickHouse();
            /* Uncomment this line for provide your custom realization of IPropertyBinder */
            /*serviceCollection.AddClickHouse(new NotImplementedPropertyBinder());*/
            
            serviceCollection.AddTransient(p => new ClickHouseConnectionSettings(connectionString));
            serviceCollection.AddTransient<ILoggerFactory, NullLoggerFactory>();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var db = serviceProvider.GetRequiredService<IClickHouseDatabase>();
            db.Open();
            for (int i = 0; i < 10; i++)
            {
                var res = db.DatabaseExists("test");
                Console.WriteLine($"Database test exists: {res}");
            }

            var queryMappingSuccess = CastRawValuesToClassTypeProperties(db);
            db.Close();
        }

        public static bool CastRawValuesToClassTypeProperties(IClickHouseDatabase db)
        {
            if (db.TableExists("Test"))
            {
                db.DropTable("Test");
            }

            db.CreateTable(new Table
            {
                Engine = "MergeTree(date, (date, Id, Number, Cost), 8192)",
                Name = "Test",
                Columns = new List<Column>()
                {
                    new Column("date", "Date"),
                    new Column("Id", "String"),
                    new Column("Number", "Int32"),
                    new Column("Cost", "Float64"),
                    new Column("Name", "String"),
                    new Column("Ushort", "UInt16"),
                    new Column("Uint", "UInt32"),

                }
            });

            var testItem = new TestDataItem3
            {
                Id = Guid.NewGuid(),
                Number = -96,
                Cost = 31,
                Name = "Jon Skeet",
                Ushort = 150,
                Uint = 65536
            };

            var command = testItem.GetInsertCommand();
            db.ExecuteNonQuery(command);

            command = "SELECT Id, Number, Cost, Name, Ushort, Uint FROM Test";
            var resultItem = db.ExecuteQueryMapping<TestDataItem3>(command, convention: new UnderscoreNamingConvention()).Single();

            db.DropTable("Test");
            return testItem.Equals(resultItem);
        }
    }
}
