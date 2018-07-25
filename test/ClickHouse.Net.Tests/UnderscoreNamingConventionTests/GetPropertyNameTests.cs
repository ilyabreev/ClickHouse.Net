using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ClickHouse.Net.Tests.UnderscoreNamingConventionTests
{
    public class GetPropertyNameTests
    {
        private readonly UnderscoreNamingConvention _target;

        public GetPropertyNameTests()
        {
            _target = new UnderscoreNamingConvention();
        }

        [Theory]
        [InlineData("max_size", "MaxSize")]
        public void ShouldConvertToPropertyName(string input, string output)
        {
            Assert.Equal(output, _target.GetPropertyName(input));
        }
    }
}
