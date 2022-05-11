using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Stenn.AspNetCore.OData.Versioning.Filters;

namespace Stenn.AspNetCore.OData.Versioning.Tests
{
    /// <summary>
    /// Test different aggregate edm model keys
    /// </summary>
    [TestFixture]
    public class EdmModelKeyAggregateTests
    {

        [TestCase("1")]
        [TestCase("1", "2")]
        [TestCase("1", "2", "3")]
        [TestCase("1", "2", "3", "4")]
        [TestCase("1", "2", "3", "4", "5")]
        [TestCase("1", "2", "3", "4", "5", "6")]
        [TestCase("1", "2", "3", "4", "5", "6", "7")]
        [TestCase("1", "2", "3", "4", "5", "6", "7", "8")]
        [TestCase("1", "2", "3", "4", "5", "6", "7", "8", "9")]
        public void EdmKeyTests(params string[] keys)
        {
            var actual1 = EdmModelKey.Aggregate(keys.Select(EdmModelKey.Get).ToArray());
            var actual2 = EdmModelKey.Aggregate(keys.Select(EdmModelKey.Get).ToArray());
            var different = EdmModelKey.Aggregate(keys.Select(k => EdmModelKey.Get(k + "_diff")).ToArray());

            actual1.Should().BeEquivalentTo(actual2);
            actual1.Should().NotBeEquivalentTo(different);
        }
    }
}