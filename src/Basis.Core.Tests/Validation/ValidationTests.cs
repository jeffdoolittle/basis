using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Basis.Validation;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Basis.Tests.Validation
{
    public class ValidationTests
    {
        [Fact]
        public void can_validate_a_simple_model()
        {
            var provider = A.Fake<IServiceProvider>();

            var engine = new ValidationEngine(provider, cfg =>
            {
                cfg.AddItem("session-id", Guid.NewGuid().ToString());
                cfg.ValidateAllProperties();
            });

            var validModel = new SimpleModel
            {
                Id = 1,
                Name = "Hello, World!"
            };

            var invalidModel = new SimpleModel();

            var validResult = engine.Validate(validModel);
            var invalidResult = engine.Validate(invalidModel);

            validResult.Count.Should().Be(0);
            invalidResult.Count.Should().Be(2);

            Action action = () => validResult.ThrowIfInvalid();

            action.Should().NotThrow();
        }

        [Fact]
        public void can_validate_a_model_with_child_objects()
        {
            var provider = A.Fake<IServiceProvider>();

            var engine = new ValidationEngine(provider, cfg =>
            {
                cfg.AddItem("session-id", Guid.NewGuid().ToString());
                cfg.ValidateAllProperties();
            });

            var validModel = new ComplexModel
            {
                Id = 1,
                Other = new SimpleModel
                {
                    Id = 2,
                    Name = "Hello, World!"
                },
                Children = new List<SimpleModel>
                {
                    new SimpleModel
                    {
                        Id = 3,
                        Name = "Hello, World!"
                    }
                }
            };

            var invalidModel = new ComplexModel
            {
                Id = 1,
                Other = new SimpleModel
                {
                    Id = 0
                },
                Children = new List<SimpleModel>
                {
                    new SimpleModel
                    {
                        Id = 0
                    }
                }
            };

            var validResult = engine.Validate(validModel);
            var invalidResult = engine.Validate(invalidModel);

            validResult.Count.Should().Be(0);
            invalidResult.Count.Should().Be(6);
        }

        [Fact]
        public void can_throw_on_invalid_result()
        {
            var provider = A.Fake<IServiceProvider>();

            var engine = new ValidationEngine(provider, cfg =>
            {
                cfg.AddItem("session-id", Guid.NewGuid().ToString());
                cfg.ValidateAllProperties();
            });

            var invalidModel = new SimpleModel();

            var invalidResult = engine.Validate(invalidModel);

            Action action = () => invalidResult.ThrowIfInvalid();

            action.Should().Throw<ValidationResultException>()
                .And
                .Results.Should().HaveCount(2);
        }

        public class SimpleModel
        {
            [Key]
            [Range(1, int.MaxValue)]
            public int Id { get; set; }

            [Required]
            public string Name { get; set; }
        }

        public class ComplexModel : IValidatableObject
        {
            [Key]
            [Range(1, int.MaxValue)]
            public int Id { get; set; }

            public SimpleModel Other { get; set; }

            public IList<SimpleModel> Children { get; set; }

            public SimpleModel[] ChildrenArray => Children.ToArray();

            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {
                var engine = validationContext.GetService<IValidationEngine>();
                var processor = new ValidationProcessor<ComplexModel>(engine);
                processor.Include(x => x.Other);
                processor.IncludeMany(x => x.Children);
                processor.IncludeMany(x => x.ChildrenArray);
                return processor.Validate(this);
            }
        }
    }
}
