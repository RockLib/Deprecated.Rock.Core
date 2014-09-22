using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Rock.Conversion
{
    internal static class ConvertFuncFactory
    {
        private static readonly ConcurrentDictionary<Tuple<Type, Type>, Func<object, object>> _convertFuncMap = new ConcurrentDictionary<Tuple<Type, Type>, Func<object, object>>();

        public static Func<object, object> GetConvertFunc(Type sourceType, Type destinationType)
        {
            return
                _convertFuncMap.GetOrAdd(
                    Tuple.Create(sourceType, destinationType),
                    tuple =>
                    {
                        if (destinationType.IsAssignableFrom(sourceType))
                        {
                            return obj => obj;
                        }

                        var conversionOperatorFunc = GetConversionOperatorFunc(sourceType, destinationType);
                        if (conversionOperatorFunc != null)
                        {
                            return conversionOperatorFunc;
                        }

                        var converter = TypeDescriptor.GetConverter(sourceType);
                        if (converter.CanConvertTo(destinationType))
                        {
                            return obj => converter.ConvertTo(obj, destinationType);
                        }

                        converter = TypeDescriptor.GetConverter(destinationType);
                        if (converter.CanConvertFrom(sourceType))
                        {
                            return obj => converter.ConvertFrom(obj);
                        }

                        if (typeof(IConvertible).IsAssignableFrom(sourceType))
                        {
                            return obj =>
                            {
                                try
                                {
                                    return Convert.ChangeType(obj, destinationType);
                                }
                                catch
                                {
                                    // Convert.ChangeType failed, so just return the object. (as with below, good luck with that!)
                                    return obj;
                                }
                            };
                        }

                        // We have no idea how to convert the value. Good luck with the raw value!
                        return obj => obj;
                    });
        }

        private static Func<object, object> GetConversionOperatorFunc(Type sourceType, Type destinationType)
        {
            var conversionOperator =
                GetConversionOperators(sourceType).Concat(GetConversionOperators(destinationType))
                    .Where(m => m.GetParameters()[0].ParameterType.IsAssignableFrom(sourceType) && destinationType.IsAssignableFrom(m.ReturnType))
                    .OrderByDescending(m => (m.GetParameters()[0].ParameterType == sourceType ? 1 : 0) + (m.ReturnType == destinationType ? 1 : 0))
                    .FirstOrDefault();

            if (conversionOperator == null)
            {
                // If is no conversion operator defined for this combination, always return null.
                return null;
            }

            var objParameter = Expression.Parameter(typeof(object), "obj");

            var lambda =
                Expression.Lambda<Func<object, object>>(
                    BoxIfNecessary(
                        Expression.Call(
                            conversionOperator,
                            Expression.Convert(objParameter, sourceType))),
                    objParameter);

            return lambda.Compile();
        }

        private static IEnumerable<MethodInfo> GetConversionOperators(Type type)
        {
            return type.GetMethods().Where(m => (m.Name == "op_Implicit" || m.Name == "op_Explicit") && m.IsStatic && m.IsSpecialName && m.GetParameters().Length == 1);
        }

        private static Expression BoxIfNecessary(Expression expression)
        {
            return
                expression.Type.IsValueType
                    ? Expression.Convert(expression, typeof(object))
                    : expression;
        }
    }
}