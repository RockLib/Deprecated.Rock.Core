using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Reflectinator;
using Rock.Extensions;

namespace Rock.Conversion
{
    public class ReflectinatorExpandoObjectConverter : IConverter<ExpandoObject>
    {
        private readonly ConcurrentDictionary<Type, Func<object, ExpandoObject>> _createFunctionCache =
            new ConcurrentDictionary<Type, Func<object, ExpandoObject>>();

        public ExpandoObject Convert(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            var createExpandoObject = _createFunctionCache.GetOrAdd(obj.GetType(), GetCreateExpandoObjectFunction);

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
                targetDictionary[sourceItem.Key] = GetValueForExpandoObject(sourceItem.Value);
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
            var getSourceItems = Method.GetFuncMethod<IEnumerable, IEnumerator>(getEnumeratorMethod).InvokeDelegate;

            var keyValuePairType =
                typeof(KeyValuePair<,>).MakeGenericType(
                    typeof(string),
                    type
                        .GetInterfaces()
                        .First(EqualsIDictionaryOfStringToAnything)
                        .GetGenericArguments()[1]);

            var getKey = Property.Get(keyValuePairType.GetProperty("Key")).GetFunc;
            var getValue = Property.Get(keyValuePairType.GetProperty("Value")).GetFunc;

            return
                obj =>
                    {
                        var expando = new ExpandoObject();

                        var targetDictionary = (IDictionary<string, object>)expando;

                        var sourceItems = getSourceItems((IEnumerable)obj);
                        while (sourceItems.MoveNext())
                        {
                            var sourceItem = sourceItems.Current;
                            targetDictionary[(string)getKey(sourceItem)] = GetValueForExpandoObject(getValue(sourceItem));
                        }

                        return expando;
                    };
        }

        private Func<object, ExpandoObject> SetValuesFromProperties(Type type)
        {
            var properties =
                TypeCrawler.Get(type)
                    .Properties
                    .Where(p => p.IsPublic && !p.IsStatic && p.CanRead)
                    .ToList();

            return
                obj =>
                {
                    var expando = new ExpandoObject();

                    var targetDictionary = (IDictionary<string, object>)expando;

                    foreach (var property in properties)
                    {
                        targetDictionary[property.Name] = GetValueForExpandoObject(property.Get(obj));
                    }

                    return expando;
                };
        }

        /// <summary>
        /// Returns an object suitable for setting the value of a "property" of an ExpandoObject. The return value for 
        /// this function will either be the object itself or an ExpandoObject that represents the object.
        /// </summary>
        private object GetValueForExpandoObject(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            if (obj.GetType().IsPrimitivish())
            {
                return obj;
            }

            return Convert(obj);
        }
    }
}