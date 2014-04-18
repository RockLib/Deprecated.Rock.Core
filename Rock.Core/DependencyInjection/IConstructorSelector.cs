using System;
using System.Reflection;

namespace Rock.DependencyInjection
{
    public interface IConstructorSelector
    {
        ConstructorInfo GetConstructor(Type type, IResolver resolver);
    }
}