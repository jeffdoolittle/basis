using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Basis.Validation
{
    public class ValidationEngine : IValidationEngine
    {
        private readonly ServiceProviderAdapter _serviceProvider;
        private readonly Dictionary<object, object> _items;
        private bool _validateAllProperties;

        public ValidationEngine(IServiceProvider serviceProvider, Action<IValidationEngineConfigurer> configure = null)
        {
            _serviceProvider = new ServiceProviderAdapter(serviceProvider, this);
            _items = new Dictionary<object, object>();
            var configurer = new ValidationEngineConfigurer(this);
            configure?.Invoke(configurer);
        }

        public interface IValidationEngineConfigurer
        {
            /// <summary>
            /// Add an Item to be used in validation
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            void AddItem(object key, object value);

            /// <summary>
            /// A value that specifies whether to validate all properties
            /// </summary>
            /// <param name="value">If false, only Required properties are validated</param>
            void ValidateAllProperties(bool value = true);
        }

        private class ValidationEngineConfigurer : IValidationEngineConfigurer
        {
            private readonly ValidationEngine _engine;

            public ValidationEngineConfigurer(ValidationEngine engine)
            {
                _engine = engine;
            }

            public void AddItem(object key, object value)
            {
                _engine._items.Add(key, value);
            }

            public void ValidateAllProperties(bool value = true)
            {
                _engine._validateAllProperties = value;
            }
        }

        public IReadOnlyList<ValidationResult> Validate(object value)
        {
            if (value == null)
            {
                return new ValidationResult[0];
            }
            var context = new ValidationContext(value, _serviceProvider, _items);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(value, context, results, _validateAllProperties);
            return results;
        }

        private class ServiceProviderAdapter : IServiceProvider
        {
            private readonly IServiceProvider _inner;
            private readonly IValidationEngine _engine;

            public ServiceProviderAdapter(IServiceProvider inner, IValidationEngine engine)
            {
                _inner = inner;
                _engine = engine;
            }

            public object GetService(Type serviceType)
            {
                return serviceType == typeof(IValidationEngine) ? _engine : _inner.GetService(serviceType);
            }
        }
    }
}
