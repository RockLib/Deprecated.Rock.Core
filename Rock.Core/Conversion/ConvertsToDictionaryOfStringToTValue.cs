using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rock.Reflection;

namespace Rock.Conversion
{
    public class ConvertsToDictionaryOfStringTo<TValue> : IConvertsTo<IDictionary<string, TValue>>
    {
        // ReSharper disable once StaticFieldInGenericType
        private static readonly ConcurrentDictionary<Type, Func<object, object>> _createDictionaryMap = new ConcurrentDictionary<Type, Func<object, object>>();

        public IDictionary<string, TValue> Convert(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            var createDictionary = _createDictionaryMap.GetOrAdd(obj.GetType(), GetCreateDictionaryFunc);

            return (IDictionary<string, TValue>)createDictionary(obj);
        }

        private static Func<object, object> GetCreateDictionaryFunc(Type t)
        {
            var dictionaryType = t.GetClosedGenericType(typeof(IDictionary<,>), new[] {typeof(string), null});
            if (dictionaryType != null)
            {
                return GetCreateDictionaryFromDictionaryFunc(dictionaryType);
            }

            return GetCreateDictionaryFromObjectPropertiesFunc(t);
        }

        private static Func<object, object> GetCreateDictionaryFromDictionaryFunc(Type dictionaryType)
        {
            if (dictionaryType.GetGenericArguments()[1] == typeof(TValue))
            {
                return x => x;
            }

            var getEnumerator = (Func<IEnumerable, IEnumerator>) Delegate.CreateDelegate(
                typeof(Func<IEnumerable, IEnumerator>),
                typeof(IEnumerable).GetMethod("GetEnumerator"));

            var keyValuePairType =
                typeof(KeyValuePair<,>).MakeGenericType(
                    typeof(string),
                    dictionaryType.GetGenericArguments()[1]);

            var getKey = keyValuePairType.GetProperty("Key").GetGetFunc<object, string>();
            var getValue = keyValuePairType.GetProperty("Value").GetGetFunc();

            // If TValue is string, we can convert by calling .ToString().
            if (typeof(TValue) == typeof(string))
            {
                getValue = GetConvertToStringFunc(getValue);
            }
            else if (typeof(TValue) != typeof(object))
            {
                getValue = GetConvertFunc(getValue);
            }

            return
                x =>
                {
                    IDictionary<string, TValue> dictionary = new Dictionary<string, TValue>();

                    var enumerator = getEnumerator((IEnumerable)x);

                    while (enumerator.MoveNext())
                    {
                        dictionary.Add(
                            getKey(enumerator.Current),
                            (TValue)getValue(enumerator.Current));
                    }

                    return dictionary;
                };
        }

        private static Func<object, object> GetCreateDictionaryFromObjectPropertiesFunc(Type t)
        {
            var properties =
                t.GetProperties()
                    .Select(p => new {p.Name, GetValue = GetPropertyValue(p)})
                    .ToArray();

            return
                x =>
                {
                    IDictionary<string, TValue> dictionary = new Dictionary<string, TValue>();

                    foreach (var property in properties)
                    {
                        dictionary.Add(property.Name, (TValue)property.GetValue(x));
                    }

                    return dictionary;
                };
        }

        private static Func<object, object> GetPropertyValue(PropertyInfo property)
        {
            var getFunc = property.GetGetFunc();

            // No extra conversion is necessary if the property type is TValue
            // or if TValue is object.
            if (property.PropertyType == typeof(TValue)
                || typeof(TValue) == typeof(object))
            {
                return getFunc;
            }

            // If TValue is string, we can convert by calling .ToString().
            if (typeof(TValue) == typeof(string))
            {
                return GetConvertToStringFunc(getFunc);
            }

            return GetConvertFunc(getFunc);
        }

        private static Func<object, object> GetConvertToStringFunc(Func<object, object> getValueFunc)
        {
            return
                obj =>
                {
                    var value = getValueFunc(obj);

                    return
                        value != null
                            ? value.ToString()
                            : null;
                };
        }

        private static Func<object, object> GetConvertFunc(Func<object, object> getValueFunc)
        {
            return
                obj =>
                {
                    var value = getValueFunc(obj);

                    if (value == null)
                    {
                        return null;
                    }

                    var convertFunc = ConvertFuncFactory.GetConvertFunc(value.GetType(), typeof(TValue));
                    return convertFunc(value);
                };
        }
    }
}