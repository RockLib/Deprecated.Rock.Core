using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rock.DependencyInjection.Heuristics
{
    public class ConstructorSelector : IConstructorSelector
    {
        public bool CanGetConstructor(Type type, IResolver resolver)
        {
            return GetConstructorImpl(type, resolver).Constructor != null;
        }

        public bool TryGetConstructor(Type type, IResolver resolver, out ConstructorInfo constructor)
        {
            var result = GetConstructorImpl(type, resolver);
            constructor = result.Constructor;
            return constructor != null;
        }

        public ConstructorInfo GetConstructor(Type type, IResolver resolver)
        {
            var result = GetConstructorImpl(type, resolver);

            if (result.Constructor != null)
            {
                return result.Constructor;
            }

            throw result.GetException();
        }

        private GetConstructorResult GetConstructorImpl(Type type, IResolver resolver)
        {
            if (type.IsAbstract)
            {
                return new GetConstructorResult
                {
                    GetException = () =>
                        new ResolveException(
                            string.Format(
                                "No resolvable constructors found for type '{0}' - abstract types cannot be instantiated.",
                                type))
                };
            }

            var rankedConstructors =
                type.GetConstructors()
                    .SelectMany(GetVirtualContructors)
                    .Where(v => v.CanResolve(resolver))
                    .GroupBy(v => v.GetScore(resolver))
                    .OrderByDescending(g => g.Key);

            var enumerator = rankedConstructors.GetEnumerator();

            if (!enumerator.MoveNext())
            {
                return new GetConstructorResult
                {
                    GetException = () =>
                        new ResolveException(
                            string.Format(
                                "No resolvable constructors found for type '{0}'.",
                                type))
                };
            }

            if (enumerator.Current.Count() > 1)
            {
                return new GetConstructorResult
                {
                    GetException = () =>
                        new ResolveException(
                            string.Format(
                                "Multiple resolvable constructors found with the same priority for type '{0}'.",
                                type))
                };
            }

            return new GetConstructorResult
            {
                Constructor = enumerator.Current.Single().Constructor
            };
        }

        private IEnumerable<VirtualContructor> GetVirtualContructors(ConstructorInfo constructor)
        {
            var parameters = constructor.GetParameters();

            var nonDefaultParameters = parameters.Where(p => !p.HasDefaultValue).Select(p => p.ParameterType).ToArray();
            var defaultParameters = parameters.Where(p => p.HasDefaultValue).Select(p => p.ParameterType).ToArray();

            return
                GetCombinations(defaultParameters)
                    .OrderByDescending(x => x.Count)
                    .Select(combination =>
                        new VirtualContructor
                        {
                            Constructor = constructor,
                            Parameters = nonDefaultParameters.Concat(combination).ToArray(),
                            DefaultParameterCount = defaultParameters.Length
                        });
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
            public ConstructorInfo Constructor { get; set; }
            public Type[] Parameters { get; set; }
            public int DefaultParameterCount { get; set; }

            public bool CanResolve(IResolver resolver)
            {
                return Parameters.All(p => resolver.CanResolve(p) && !IsPrimitivish(p));
            }

            public int GetScore(IResolver resolver)
            {
                return (100 * Parameters.Length) - DefaultParameterCount;
            }

            private static bool IsPrimitivish(Type type)
            {
                return
                    type.IsPrimitive
                    || type == typeof(string)
                    || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) && type.GetGenericArguments()[0].IsPrimitive);
            }
        }

        private class GetConstructorResult
        {
            public ConstructorInfo Constructor { get; set; }
            public Func<Exception> GetException { get; set; }
        }
    }
}