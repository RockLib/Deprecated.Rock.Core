using System.Collections.Generic;
using System.Linq;

namespace Rock.Collections
{
    public static class ConcatExtensions
    {
        public static IEnumerable<T> Concat<T>(this T instance, IEnumerable<T> items)
        {
            return Enumerable.Repeat(instance, 1).Concat(items);
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> items, params T[] additionalItems)
        {
            return Enumerable.Concat(items, additionalItems);
        }
    }
}