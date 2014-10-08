using System;
using System.Reflection;

namespace Rock.DependencyInjection.Heuristics
{
    public static class ResolverConstructorSelectorExtensions
    {
        public static bool CanGetConstructor(this IResolverConstructorSelector constructorSelector, Type type, IResolver resolver)
        {
            ConstructorInfo dummy;
            return constructorSelector.TryGetConstructor(type, resolver, out dummy);
        }

        public static ConstructorInfo GetConstructor(this IResolverConstructorSelector constructorSelector, Type type, IResolver resolver)
        {
            ConstructorInfo ctor;
            if (constructorSelector.TryGetConstructor(type, resolver, out ctor))
            {
                return ctor;
            }

            throw new ResolveException("Unable to resolve type: " + type);
        }
    }
}