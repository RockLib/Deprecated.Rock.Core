using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Reflectinator;

namespace Rock.Conversion
{
    public class ReflectinatorConvertToExpandoObject : IConvertTo<ExpandoObject>
    {
        private readonly ConcurrentDictionary<Type, Func<object, ExpandoObject>> _createFunctionCache =
            new ConcurrentDictionary<Type, Func<object, ExpandoObject>>();

        private readonly ConcurrentDictionary<Type, bool> _isPrimitivishOrNullablePrimitivishCache =
            new ConcurrentDictionary<Type, bool>(); 

        public ExpandoObject Convert(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            var createExpandoObject =
                _createFunctionCache.GetOrAdd(
                    obj.GetType(),
                    GetCreateFunction);

            return createExpandoObject(obj);
        }

        private Func<object, ExpandoObject> GetCreateFunction(Type type)
        {
            if (IsPrimitivishOrNullablePrimitivish(type))
            {
                throw new InvalidOperationException(string.Format("Cannot convert type '{0}' to ExpandoObject.", type));
            }

            if (IsExpandoObject(type))
            {
                return CastToExpandoObject;
            }

            if (IsIDictionaryOfStringToObject(type))
            {
                return CopyDictionaryItems;
            }

            if (IsIDictionaryOfStringToAnything(type))
            {
                return GetFuncCopyDictionaryItemsViaReflectinator(type);
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

        private ExpandoObject CopyDictionaryItems(object obj)
        {
            var expando = new ExpandoObject();

            var sourceDictionary = (IDictionary<string, object>)obj;
            var targetDictionary = (IDictionary<string, object>)expando;

            foreach (var x in sourceDictionary)
            {
                targetDictionary[x.Key] = GetValueForExpandoObject(x.Value);
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

        private Func<object, ExpandoObject> GetFuncCopyDictionaryItemsViaReflectinator(Type type)
        {
            var getEnumeratorMethod = typeof(IEnumerable).GetMethod("GetEnumerator");
            var getEnumerator = Method.GetFuncMethod<IEnumerable, IEnumerator>(getEnumeratorMethod).InvokeDelegate;

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

                        var enumerator = getEnumerator((IEnumerable)obj);
                        while (enumerator.MoveNext())
                        {
                            var x = enumerator.Current;
                            targetDictionary[(string)getKey(x)] = GetValueForExpandoObject(getValue(x));
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
                    var dictionary = (IDictionary<string, object>)expando;

                    foreach (var property in properties)
                    {
                        dictionary[property.Name] = GetValueForExpandoObject(property.Get(obj));
                    }

                    return expando;
                };
        }

        /// <summary>
        /// Returns an object suitable for setting the value of a "property" of an ExpandoObject. The return value for 
        /// this function will either be the input arg itself, or an ExpandoObject that represents the input arg.
        /// </summary>
        private object GetValueForExpandoObject(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            if (IsPrimitivishOrNullablePrimitivish(obj.GetType()))
            {
                return obj;
            }

            return Convert(obj);
        }

        private bool IsPrimitivishOrNullablePrimitivish(Type type)
        {
            return _isPrimitivishOrNullablePrimitivishCache.GetOrAdd(
                type,
                t =>
                    IsPrimitivish(t)
                    || (t.IsGenericType
                        && t.GetGenericTypeDefinition() == typeof(Nullable<>)
                        && IsPrimitivish(t.GetGenericArguments()[0])));
        }

        private static bool IsPrimitivish(Type type)
        {
            return type.IsPrimitive || type == typeof(string) || type == typeof(decimal);
        }
    }
}