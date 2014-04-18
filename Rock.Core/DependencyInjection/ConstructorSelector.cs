using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rock.DependencyInjection
{
    public class ConstructorSelector : IConstructorSelector
    {
        public ConstructorInfo GetConstructor(Type type, IResolver resolver)
        {
            var rankedConstructors =
                type.GetConstructors()
                    .SelectMany(GetVirtualContructors)
                    .Where(v => v.CanResolve(resolver))
                    .GroupBy(v => v.GetScore(resolver))
                    .OrderByDescending(g => g.Key);

            var enumerator = rankedConstructors.GetEnumerator();

            if (!enumerator.MoveNext())
            {
                // TODO: better exception type and message
                throw new Exception("no constructor found");
            }

            if (enumerator.Current.Count() > 1)
            {
                // TODO: better exception type and message
                throw new Exception("multiple constructors with the same priority found");
            }

            return enumerator.Current.Single().Constructor;
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
    }
}