using System;

namespace Rock.Defaults
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class IsDefaultProviderAttribute : Attribute
    {
    }
}