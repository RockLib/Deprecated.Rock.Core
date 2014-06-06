using System;
using System.Collections.Concurrent;
using System.Linq;
using Rock.Defaults.Implementation;

namespace Rock.Extensions
{
    public static class IsPrimitivishExtension
    {
        private static readonly ConcurrentDictionary<Type, bool> _cache = new ConcurrentDictionary<Type, bool>(); 

        public static bool IsPrimitivish(this Type type)
        {
            return _cache.GetOrAdd(type, t => t.IsNonNullablePrimitivish() || t.IsNullablePrimitivish());
        }

        internal static void ClearCache()
        {
            _cache.Clear();
        }

        private static bool IsNonNullablePrimitivish(this Type type)
        {
            return
                type.IsPrimitive
                || type.IsEnum
                || Default.PrimitivishTypes.Any(t => t == type);
        }

        private static bool IsNullablePrimitivish(this Type type)
        {
            return
                type.IsGenericType
                && type.GetGenericTypeDefinition() == typeof(Nullable<>)
                && type.GetGenericArguments()[0].IsNonNullablePrimitivish();
        }
    }
}