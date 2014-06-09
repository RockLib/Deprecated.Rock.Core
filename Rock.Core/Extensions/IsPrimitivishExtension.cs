using System;
using System.Collections.Generic;
using System.Linq;
using Rock.Defaults;
using Rock.Defaults.Implementation;

namespace Rock.Extensions
{
    public static class IsPrimitivishExtension
    {
        internal static readonly Type[] _defaultPrimitivishTypes =
        {
            typeof(string),
            typeof(decimal),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(Guid),
            typeof(TimeSpan)
        };

        [UsesDefaultValue(typeof(Default), "ExtraPrimitivishTypes")]
        public static bool IsPrimitivish(this Type type, IEnumerable<Type> extraPrimitivishTypes = null)
        {
            var primitivishTypeList = _defaultPrimitivishTypes.Concat(extraPrimitivishTypes ?? Default.ExtraPrimitivishTypes).ToList();

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