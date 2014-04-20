using System;
using System.Reflection;

namespace Rock.DependencyInjection.Heuristics
{
    public interface IConstructorSelector
    {
        bool CanGetConstructor(Type type, IResolver resolver);
        bool TryGetConstructor(Type type, IResolver resolver, out ConstructorInfo constructor);
        ConstructorInfo GetConstructor(Type type, IResolver resolver);
    }
}