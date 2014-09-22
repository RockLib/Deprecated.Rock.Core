using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Rock.Defaults;
using Rock.Defaults.Implementation;
using Rock.Extensions;
using Rock.Reflection;

namespace Rock.Conversion
{
    public class ConvertsToExpandoObject : IConvertsTo<ExpandoObject>
    {
        private readonly ConcurrentDictionary<Type, Func<object, ExpandoObject>> _createExpandoObjectFunctions = new ConcurrentDictionary<Type, Func<object, ExpandoObject>>();
        private readonly ConcurrentDictionary<Type, Func<object, object>> _getPropertyValueFunctions = new ConcurrentDictionary<Type, Func<object, object>>();

        [UsesDefaultValue(typeof(Default), "ExtraPrimitivishTypes")]
        public ExpandoObject Convert(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            var createExpandoObject = _createExpandoObjectFunctions.GetOrAdd(obj.GetType(), GetCreateExpandoObjectFunction);

            return createExpandoObject(obj);
        }

        private Func<object, ExpandoObject> GetCreateExpandoObjectFunction(Type type)
        {
            if (type.IsPrimitivish())
            {
                throw new InvalidOperationException(string.Format("Cannot convert type '{0}' to ExpandoObject.", type));
            }

            if (IsExpandoObject(type))
            {
                return CastToExpandoObject;
            }

            if (IsIDictionaryOfStringToObject(type))
            {
                return SetValuesFromIDictionaryOfStringToObjectItems;
            }

            if (IsIDictionaryOfStringToAnything(type))
            {
                return SetValuesFromIDictionaryOfStringToAnythingItems(type);
            }

            return SetValuesFromProperties(type);
        }

        private static bool IsExpandoObject(Type type)
        {
            return type == typeof(ExpandoObject);
        }

        private static ExpandoObject CastToExpandoObject(object obj)
        {
            return (ExpandoObject)obj;
        }

        private static bool IsIDictionaryOfStringToObject(Type type)
        {
            return type
                .GetInterfaces()
                .Any(i => i == typeof(IDictionary<string, object>));
        }

        private ExpandoObject SetValuesFromIDictionaryOfStringToObjectItems(object obj)
        {
            var expando = new ExpandoObject();

            var sourceDictionary = (IDictionary<string, object>)obj;
            var targetDictionary = (IDictionary<string, object>)expando;

            foreach (var sourceItem in sourceDictionary)
            {
                targetDictionary[sourceItem.Key] = GetPropertyValue(sourceItem.Value);
            }

            return expando;
        }

        private static bool IsIDictionaryOfStringToAnything(Type type)
        {
            return type.GetInterfaces().Any(EqualsIDictionaryOfStringToAnything);
        }

        private static bool EqualsIDictionaryOfStringToAnything(Type type)
        {
            return type.IsGenericType
                   && type.GetGenericTypeDefinition() == typeof(IDictionary<,>)
                   && type.GetGenericArguments()[0] == typeof(string);
        }

        private Func<object, ExpandoObject> SetValuesFromIDictionaryOfStringToAnythingItems(Type type)
        {
            var getEnumeratorMethod = typeof(IEnumerable).GetMethod("GetEnumerator");
            var getSourceItems =
                (Func<IEnumerable, IEnumerator>)Delegate.CreateDelegate(
                    typeof(Func<IEnumerable, IEnumerator>),
                    getEnumeratorMethod);

            var keyValuePairType =
                typeof(KeyValuePair<,>).MakeGenericType(
                    typeof(string),
                    type
                        .GetInterfaces()
                        .First(EqualsIDictionaryOfStringToAnything)
                        .GetGenericArguments()[1]);

            var getKey = keyValuePairType.GetProperty("Key").GetGetFunc();
            var getValue = keyValuePairType.GetProperty("Value").GetGetFunc();

            return
                obj =>
                    {
                        var expando = new ExpandoObject();

                        var targetDictionary = (IDictionary<string, object>)expando;

                        var sourceItems = getSourceItems((IEnumerable)obj);
                        while (sourceItems.MoveNext())
                        {
                            var sourceItem = sourceItems.Current;
                            targetDictionary[(string)getKey(sourceItem)] = GetPropertyValue(getValue(sourceItem));
                        }

                        return expando;
                    };
        }

        private Func<object, ExpandoObject> SetValuesFromProperties(Type type)
        {
            var properties =
                type.GetProperties()
                    .Where(p => p.IsPublic() && !p.IsStatic() && p.CanRead)
                    .Select(p => new { p.Name, Get = p.GetGetFunc() })
                    .ToList();

            return
                obj =>
                {
                    var expando = new ExpandoObject();

                    var targetDictionary = (IDictionary<string, object>)expando;

                    foreach (var property in properties)
                    {
                        targetDictionary[property.Name] = GetPropertyValue(property.Get(obj));
                    }

                    return expando;
                };
        }

        /// <summary>
        /// Returns an object suitable for setting the value of a "property" of an ExpandoObject. The return value for 
        /// this function will either be the object itself or an ExpandoObject that represents the object.
        /// </summary>
        private object GetPropertyValue(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            var getPropertyValue = _getPropertyValueFunctions.GetOrAdd(obj.GetType(), GetGetPropertyValueFunction);

            return getPropertyValue(obj);
        }

        private Func<object, object> GetGetPropertyValueFunction(Type t)
        {
            if (t.IsPrimitivish())
            {
                return o => o;
            }

            return _createExpandoObjectFunctions.GetOrAdd(t, GetCreateExpandoObjectFunction);
        }
    }
}