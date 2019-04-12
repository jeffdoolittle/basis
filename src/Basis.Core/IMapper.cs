using System.Collections.Generic;
using System.Linq;

namespace Basis
{
    public interface IMapper<in TFrom, in TTo>
    {
        void Map(TFrom source, TTo destination);
    }

    public static class MapperExtensions
    {
        public static TTo Map<TFrom, TTo>(this IMapper<TFrom, TTo> mapper, TFrom source) where TTo : new()
        {
            Guard.NotNull(mapper, nameof(mapper));
            Guard.NotNull(source, nameof(source));

            var destination = Factory<TTo>.CreateInstance();
            mapper.Map(source, destination);
            return destination;
        }

        public static IReadOnlyList<TTo> Map<TFrom, TTo>(this IMapper<TFrom, TTo> mapper, IEnumerable<TFrom> source) where TTo : new()
        {
            return source.Select(mapper.Map).ToList();
        }

        public static IReadOnlyList<TTo> Map<TFrom, TTo>(this IMapper<TFrom, TTo> mapper, IList<TFrom> source) where TTo : new()
        {
            var destination = new TTo[source.Count];
            for (var i = 0; i < source.Count; ++i)
            {
                var sourceItem = source[i];
                var destinationItem = Factory<TTo>.CreateInstance();
                mapper.Map(sourceItem, destinationItem);
                destination[i] = destinationItem;
            }

            return destination;
        }
    }
}