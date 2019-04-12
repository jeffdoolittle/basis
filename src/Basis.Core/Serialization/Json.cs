using System;

namespace Basis.Serialization
{
    public interface IJsonSerializer
    {
        string Serialize(object value);
        T Deserialize<T>(string value);
        object Deserialize(Type type, string value);
    }

    public class Json
    {
        public static IJsonSerializer Serializer => new Lazy<IJsonSerializer>(() => new JsonSerializer()).Value;

        private class JsonSerializer : IJsonSerializer
        {
            public string Serialize(object value)
            {
                return SimpleJson.SerializeObject(value, new JsonSerializerStrategy());
            }

            public T Deserialize<T>(string value)
            {
                return (T)Deserialize(typeof(T), value);
            }

            public object Deserialize(Type type, string value)
            {
                return SimpleJson.DeserializeObject(value, type, new JsonSerializerStrategy());
            }
        }
        
        private class JsonSerializerStrategy : IJsonSerializerStrategy
        {
            private readonly IJsonSerializerStrategy _inner;

            public JsonSerializerStrategy()
            {
                _inner = new PocoJsonSerializerStrategy();
            }

            public bool TrySerializeNonPrimitiveObject(object input, out object output)
            {
                if (input is DateTime dt)
                {
                    if (dt.Kind != DateTimeKind.Utc)
                    {
                        throw new InvalidOperationException($"DateTime.Kind must be {DateTimeKind.Utc}. {DateTimeKind.Local} and {DateTimeKind.Unspecified} are not allowed.");
                    }
                }

                return _inner.TrySerializeNonPrimitiveObject(input, out output);
            }

            public object DeserializeObject(object value, Type type)
            {
                return _inner.DeserializeObject(value, type);
            }
        }
    }
}
