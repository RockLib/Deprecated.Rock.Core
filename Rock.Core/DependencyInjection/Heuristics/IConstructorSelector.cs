using System;
using System.Reflection;

namespace Rock.DependencyInjection.Heuristics
{
    public interface IConstructorSelector
    {
        bool TryGetConstructor(Type type, IResolver resolver, out ConstructorInfo constructor);
    }
}