using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;

namespace Basis.Validation
{
    public class ValidationProcessor<T>
    {
        private readonly IValidationEngine _engine;
        private readonly List<PropertyDescriptor> _properties;
        private readonly List<MultiPropertyDescriptor> _multiProperties;

        public ValidationProcessor(IValidationEngine engine)
        {
            _engine = engine;
            _properties = new List<PropertyDescriptor>();
            _multiProperties = new List<MultiPropertyDescriptor>();
        }

        public ValidationProcessor<T> Include<TProperty>(Expression<Func<T, TProperty>> property)
        {
            var func = property.Compile();

            _properties.Add(new PropertyDescriptor
            {
                PropertyTypeName = typeof(TProperty).Name,
                PropertyName = Expressions.GetPropertyName(property),
                PropertyFunc = _ => func((T)_)
            });
            return this;
        }

        public ValidationProcessor<T> IncludeMany<TProperty>(Expression<Func<T, TProperty[]>> property)
        {
            var func = property.Compile();

            _multiProperties.Add(new MultiPropertyDescriptor
            {
                PropertyTypeName = typeof(TProperty).Name,
                PropertyName = Expressions.GetPropertyName(property),
                PropertyFunc = _ => func((T)_).Cast<object>().ToArray()
            });
            return this;
        }

        public ValidationProcessor<T> IncludeMany<TProperty>(Expression<Func<T, IEnumerable<TProperty>>> property)
        {
            var func = property.Compile();

            _multiProperties.Add(new MultiPropertyDescriptor
            {
                PropertyTypeName = typeof(TProperty).Name,
                PropertyName = Expressions.GetPropertyName(property),
                PropertyFunc = _ => func((T)_).Cast<object>().ToArray()
            });
            return this;
        }

        public IReadOnlyList<ValidationResult> Validate(T value)
        {
            var results = new List<ValidationResult>();

            foreach (var property in _properties)
            {
                var propertyResults = _engine
                    .Validate(property.PropertyFunc(value))
                    .Select(r => new ValidationResult($"{property.PropertyName} ({property.PropertyTypeName}): {r.ErrorMessage}", r.MemberNames));
                results.AddRange(propertyResults);
            }
            foreach (var property in _multiProperties)
            {
                var childValues = property.PropertyFunc(value);
                for (var c = 0; c < childValues.Length; c++)
                {
                    var index = c;
                    var child = childValues[c];
                    var propertyResults = _engine
                        .Validate(child)
                        .Select(r => new ValidationResult($"{property.PropertyName} ({property.PropertyTypeName}) at Index {index}: {r.ErrorMessage}", r.MemberNames));
                    results.AddRange(propertyResults);
                }
            }

            return results;
        }

        private class PropertyDescriptor
        {
            public string PropertyTypeName { get; set; }
            public string PropertyName { get; set; }
            public Func<object, object> PropertyFunc { get; set; }
        }

        private class MultiPropertyDescriptor
        {
            public string PropertyTypeName { get; set; }
            public string PropertyName { get; set; }
            public Func<object, object[]> PropertyFunc { get; set; }
        }
    }
}