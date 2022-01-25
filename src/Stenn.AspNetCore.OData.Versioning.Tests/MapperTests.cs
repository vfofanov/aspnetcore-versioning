using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.OData.Formatter;
using NUnit.Framework;
using Stenn.AspNetCore.OData.Versioning.Actions;

namespace Stenn.AspNetCore.OData.Versioning.Tests
{
    [TestFixture]
    public class MapperTests
    {
        private sealed class Params: ODataActionParams
        {
            // ReSharper disable UnusedAutoPropertyAccessor.Local
            public int Id { get; set; }
            public string Test { get; set; }
            public IEnumerable<int> Ids { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Local


        }

        [Test]
        public void ReadValues()
        {
            var id = 10;
            var test = "Test";
            var ids = new[] { 1, 2, 3 };
            
            var parameters = new ODataActionParameters
            {
                { "Test", test },
                { "Id", id },
                { "Ids", ids }
            };

            var result = parameters.Get<Params>();

            result.Test.Should().Be(test);
            result.Id.Should().Be(id);
            result.Ids.Should().Equal(ids);
        }
        
        [Test]
        public void ReadValuesCamelCase()
        {
            var id = 10;
            var test = "Test";
            var ids = new[] { 1, 2, 3 };
            
            var parameters = new ODataActionParameters
            {
                { "test", test },
                { "id", id },
                { "ids", ids }
            };

            var result = parameters.Get<Params>();

            result.Test.Should().Be(test);
            result.Id.Should().Be(id);
            result.Ids.Should().Equal(ids);
        }

        [Test]
        public void ReadPartValues()
        {
            var id = 10;
            var test = "Test";

            var parameters = new ODataActionParameters
            {
                { "Test", test },
                { "Id", id },
            };

            var result = parameters.Get<Params>();

            result.Test.Should().Be(test);
            result.Id.Should().Be(id);
            result.Ids.Should().Equal(null);
        }
    }
}