using System;
using System.Collections.Generic;
using Xunit;

namespace ClickHouse.Net.Tests
{
    public class ClickHouseDataFormatterTests
    {
        public ClickHouseDataFormatterTests()
        {
            Target = new ClickHouseDataFormatter();
        }

        public ClickHouseDataFormatter Target { get; set; }

        [Fact]
        public void Convert_Result_From_Raw_Data()
        {
            // act & assert
            TestConversionFromRawData(GetRawData());
        }

        [Fact]
        public void Convert_Result_From_Raw_Data_With_NaNs()
        {
            // act & assert
            TestConversionFromRawData(GetRawDataWithNaNs());
        }

        [Fact]
        public void Throw_Exception_If_Properties_Count_Doesnt_Equal_Columns_Count()
        {
            // act & assert
            Assert.Throws<InvalidOperationException>(() => Target.ConvertFromRawData<TestDataItem2>(GetRawData()));
        }

        private void TestConversionFromRawData(object[][] rawData)
        {
            // arrange
            var expected = GetResutFromRawData(rawData);

            // act
            var result = Target.ConvertFromRawData<TestDataItem>(rawData);

            // assert
            Assert.NotEmpty(result);
            Assert.Equal(expected, result);
        }

        private object[][] GetRawData()
        {
            return new object[][]
            {
                new object[] { "Id1", 0.56 },
                new object[] { "Id2", 2 },
                new object[] { "Id3", (long)7 },
                new object[] { "Id4", 3.43 }
            };
        }

        private object[][] GetRawDataWithNaNs()
        {
            return new object[][]
            {
                new object[] { "Id1", 0.55 },
                new object[] { "Id2", "NaN" },
                new object[] { "Id3", "inf" },
                new object[] { "Id4", "infinity"}
            };
        }

        private IEnumerable<TestDataItem> GetResutFromRawData(object[][] rawData)
        {
            foreach (var row in rawData)
            {
                yield return new TestDataItem
                {
                    Id = (string)row[0],
                    Value = !"NaN".Equals(row[1]) && double.TryParse(row[1].ToString(), out var value) ? value : 0
                };
            }

        }
    }
}
