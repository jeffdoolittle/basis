using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Basis.Tests
{
    public class MapperTests
    {
        [Fact]
        public void can_map_single_item_to_existing_target()
        {
            var source = new Source { Name = "foobar" };
            var destination = new Destination();
            new Mapper().Map(source, destination);
            destination.Name.Should().Be(source.Name);
        }

        [Fact]
        public void can_map_single_item_to_created_target()
        {
            var source = new Source { Name = "foobar" };
            var destination = new Mapper().Map(source);
            destination.Name.Should().Be(source.Name);
        }

        [Fact]
        public void can_map_enumerable()
        {
            var source = Enumerable.Range(0, 1)
                .Select(_ => new Source { Name = "foobar" });
            var destination = new Mapper().Map(source);
            destination[0].Name.Should().Be(source.First().Name);
        }

        [Fact]
        public void can_map_array()
        {
            var source = new[] { new Source { Name = "foobar" } };
            var destination = new Mapper().Map(source);
            destination[0].Name.Should().Be(source[0].Name);
        }

        [Fact]
        public void can_map_list()
        {
            var source = new List<Source> { new Source { Name = "foobar" } };
            var destination = new Mapper().Map(source);
            destination[0].Name.Should().Be(source[0].Name);
        }

        public class Mapper : IMapper<Source, Destination>
        {
            public void Map(Source source, Destination destination)
            {
                destination.Name = source.Name;
            }
        }

        public class Source
        {
            public string Name { get; set; }
        }

        public class Destination
        {
            public string Name { get; set; }
        }
    }
}