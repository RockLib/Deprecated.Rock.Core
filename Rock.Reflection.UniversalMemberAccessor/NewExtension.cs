using System;
using Microsoft.CSharp.RuntimeBinder;

namespace Rock.Reflection
{
    /// <summary>
    /// A class to provide the <see cref="New"/> extension method.
    /// </summary>
    public static class NewExtension
    {
        /// <summary>
        /// Initializes a new instance of the type specified by the <paramref name="type"/>
        /// parameter using constructor arguments specified by the <paramref name="args"/>
        /// parameter.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to create.</param>
        /// <param name="args">Constructor arguments to pass to the constructor.</param>
        /// <returns>
        /// A dynamic proxy object enabling access to all members of the newly created
        /// object of type <paramref name="type"/>.
        /// </returns>
        public static dynamic New(this Type type, params object[] args)
        {
            UniversalMemberAccessor factory = UniversalMemberAccessor.GetStatic(type);
            var createInstace = factory.GetCreateInstanceFunc(args);
            return createInstace(args);
        }
    }
}