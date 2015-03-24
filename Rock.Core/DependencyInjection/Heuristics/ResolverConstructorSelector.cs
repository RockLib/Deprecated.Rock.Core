using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rock.Reflection;

namespace Rock.DependencyInjection.Heuristics
{
    public class ResolverConstructorSelector : IResolverConstructorSelector
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

        private static IEnumerable<VirtualConstructor> GetVirtualContructors(ConstructorInfo constructor)
        {
            var parameters = constructor.GetParameters();

            var nonDefaultParameters = parameters.Where(p => !p.HasDefaultValue).Select(p => p.ParameterType).ToArray();
            var defaultParameters = parameters.Where(p => p.HasDefaultValue).Select(p => p.ParameterType).ToArray();

            return
                GetCombinations(defaultParameters)
                    .OrderByDescending(x => x.Count)
                    .Select(combination =>
                        new VirtualConstructor(constructor, nonDefaultParameters.Concat(combination).ToArray(), defaultParameters.Length));
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

        private class VirtualConstructor
        {
            private readonly Type[] _parameters;
            private readonly int _defaultParameterCount;

            public VirtualConstructor(ConstructorInfo constructor, Type[] parameters, int defaultParameterCount)
            {
                Constructor = constructor;
                _parameters = parameters;
                _defaultParameterCount = defaultParameterCount;
            }

            public ConstructorInfo Constructor { get; private set; }

            public bool CanResolve(IResolver resolver)
            {
                return _parameters.All(p => resolver.CanGet(p) && !p.IsPrimitivish());
            }

            public int GetScore()
            {
                return (100 * _parameters.Length) - _defaultParameterCount;
            }
        }
    }
}