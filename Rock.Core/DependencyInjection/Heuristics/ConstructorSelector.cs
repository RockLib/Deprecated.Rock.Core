using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rock.DependencyInjection.Heuristics
{
    public class ConstructorSelector : IConstructorSelector
    {
        public bool TryGetConstructor(Type type, IResolver resolver, out ConstructorInfo constructor)
        {
            if (type.IsAbstract)
            {
                constructor = null;
                return false;
            }

            var rankedConstructors =
                type.GetConstructors()
                    .SelectMany(GetVirtualContructors)
                    .Where(v => v.CanResolve(resolver))
                    .GroupBy(v => v.GetScore())
                    .OrderByDescending(g => g.Key);

            var enumerator = rankedConstructors.GetEnumerator();

            if (!enumerator.MoveNext())
            {
                constructor = null;
                return false;
            }

            if (enumerator.Current.Count() > 1)
            {
                constructor = null;
                return false;
            }

            constructor = enumerator.Current.Single().Constructor;
            return true;
        }

        private static IEnumerable<VirtualContructor> GetVirtualContructors(ConstructorInfo constructor)
        {
            var parameters = constructor.GetParameters();

            var nonDefaultParameters = parameters.Where(p => !p.HasDefaultValue).Select(p => p.ParameterType).ToArray();
            var defaultParameters = parameters.Where(p => p.HasDefaultValue).Select(p => p.ParameterType).ToArray();

            return
                GetCombinations(defaultParameters)
                    .OrderByDescending(x => x.Count)
                    .Select(combination =>
                        new VirtualContructor(constructor, nonDefaultParameters.Concat(combination).ToArray(), defaultParameters.Length));
        }

        private static IEnumerable<IList<Type>> GetCombinations(IList<Type> types)
        {
            double count = Math.Pow(2, types.Count);
            for (int i = 0; i < count; i++)
            {
                var returnList = new List<Type>();

                string str = Convert.ToString(i, 2).PadLeft(types.Count, '0');

                for (int j = 0; j < str.Length; j++)
                {
                    if (str[j] == '1')
                    {
                        returnList.Add(types[j]);
                    }
                }

                yield return returnList;
            }
        }

        private class VirtualContructor
        {
            private readonly Type[] _parameters;
            private readonly int _defaultParameterCount;

            public VirtualContructor(ConstructorInfo constructor, Type[] parameters, int defaultParameterCount)
            {
                Constructor = constructor;
                _parameters = parameters;
                _defaultParameterCount = defaultParameterCount;
            }

            public ConstructorInfo Constructor { get; private set; }

            public bool CanResolve(IResolver resolver)
            {
                return _parameters.All(p => resolver.CanGet(p) && !IsPrimitivish(p));
            }

            public int GetScore()
            {
                return (100 * _parameters.Length) - _defaultParameterCount;
            }

            private static bool IsPrimitivish(Type type)
            {
                return
                    IsNonNullablePrimitivish(type)
                    || IsNullablePrimitivish(type);
            }

            private static bool IsNonNullablePrimitivish(Type type)
            {
                return
                    type.IsPrimitive
                    || type == typeof(string)
                    || type == typeof(decimal)
                    || type == typeof(DateTime)
                    || type == typeof(DateTimeOffset)
                    || type == typeof(Guid)
                    || type == typeof(TimeSpan);
            }

            private static bool IsNullablePrimitivish(Type type)
            {
                return
                    type.IsGenericType
                    && type.GetGenericTypeDefinition() == typeof(Nullable<>)
                    && IsNonNullablePrimitivish(type.GetGenericArguments()[0]);
            }
        }
    }
}