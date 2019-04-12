using System;
using Basis.Serialization;
using FluentAssertions;
using Xunit;

namespace Basis.Tests.Serialization
{
    public class JsonSerializerTests
    {
        [Fact]
        public void can_serialize_and_deserialize_object()
        {
            var input = Init(DateTime.UtcNow);

            var json = Json.Serializer.Serialize(input);

            var output = Json.Serializer.Deserialize<Foo>(json);

            output.Text.Should().Be(input.Text);

            output.DateTime.Should().Be(input.DateTime);

            output.UtcNowOffset.Should().Be(input.UtcNowOffset);
            output.LocalNowOffset.Should().Be(input.LocalNowOffset);

            // local date time offsets are shifted to utc
            output.LocalNowOffset.Offset.Should().Be(TimeSpan.Zero);
        }

        [Fact]
        public void exception_serializing_object_with_local_date_time()
        {
            var input = Init(DateTime.Now);

            Action action = () =>  Json.Serializer.Serialize(input);

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void exception_serializing_object_with_unspecified_date_time()
        {
            var input = Init(new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Unspecified));

            Action action = () => Json.Serializer.Serialize(input);

            action.Should().Throw<InvalidOperationException>();
        }

        private Foo Init(DateTime dt)
        {
            var foo = new Foo();

            foo.Text = Guid.NewGuid().ToString();
            foo.DateTime = dt;

            foo.UtcNowOffset = DateTimeOffset.UtcNow;
            foo.LocalNowOffset = DateTimeOffset.Now;

            return foo;
        }

        private class Foo
        {
            public string Text { get; set; }
            public DateTime DateTime { get; set; }

            public DateTimeOffset UtcNowOffset { get; set; }
            public DateTimeOffset LocalNowOffset { get; set; }
        }
    }
}
