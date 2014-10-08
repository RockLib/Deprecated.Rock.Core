using System;
using System.Reflection;

namespace Rock.DependencyInjection.Heuristics
{
    public interface IResolverConstructorSelector
    {
        bool TryGetConstructor(Type type, IResolver resolver, out ConstructorInfo constructor);
    }
}