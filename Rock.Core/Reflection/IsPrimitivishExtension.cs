using System;
using System.Collections.Generic;
using System.Linq;
using Rock.Immutable;

namespace Rock.Reflection
{
    public static class IsPrimitivishExtension
    {
        private static readonly Semimutable<IEnumerable<Type>> _extraPrimitivishTypes = new Semimutable<IEnumerable<Type>>(GetDefaultExtraPrimitiveTypes);

        internal static readonly Type[] _defaultPrimitivishTypes =
        {
            typeof(string),
            typeof(decimal),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(Guid),
            typeof(TimeSpan)
        };

        public static IEnumerable<Type> ExtraPrimitivishTypes
        {
            get { return _extraPrimitivishTypes.Value; }
        }

        public static void SetExtraPrimitivishTypes(IEnumerable<Type> extraPrimitivishTypes)
        {
            _extraPrimitivishTypes.Value = extraPrimitivishTypes;
        }

        internal static void ResetExtraPrimitivishTypes()
        {
            UnlockExtraPrimitivishTypes();
            _extraPrimitivishTypes.ResetValue();
        }

        internal static void UnlockExtraPrimitivishTypes()
        {
            _extraPrimitivishTypes.UnlockValue();
        }

        private static IEnumerable<Type> GetDefaultExtraPrimitiveTypes()
        {
            return Enumerable.Empty<Type>();
        }

        public static bool IsPrimitivish(this Type type, IEnumerable<Type> extraPrimitivishTypes = null)
        {
            var primitivishTypeList = _defaultPrimitivishTypes.Concat(extraPrimitivishTypes ?? _extraPrimitivishTypes.Value).ToList();

            return
                type.IsNonNullablePrimitivish(primitivishTypeList)
                || type.IsNullablePrimitivish(primitivishTypeList);
        }

        private static bool IsNonNullablePrimitivish(this Type type, IEnumerable<Type> primitivishTypes)
        {
            return
                type.IsPrimitive
                || type.IsEnum
                || primitivishTypes.Any(t => t == type);
        }

        private static bool IsNullablePrimitivish(this Type type, IEnumerable<Type> primitivishTypes)
        {
            return
                type.IsGenericType
                && type.GetGenericTypeDefinition() == typeof(Nullable<>)
                && type.GetGenericArguments()[0].IsNonNullablePrimitivish(primitivishTypes);
        }
    }
}