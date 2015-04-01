using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rock.Reflection
{
    /// <summary>
    /// Provides a <see cref="GetTypesSafely"/> extension method.
    /// </summary>
    public static class GetTypesSafelyExtension
    {
        private static readonly ConcurrentDictionary<Assembly, IReadOnlyCollection<Type>> _assemblyTypes = new ConcurrentDictionary<Assembly, IReadOnlyCollection<Type>>();

        /// <summary>
        /// Gets the types defined in the source assembly safely.
        /// </summary>
        /// <param name="assembly">The assembly to get types from.</param>
        /// <returns>
        /// A collection containing all the types that are defined (and successfully
        /// loaded) from the given assembly.
        /// </returns>
        /// <remarks>
        /// It is possible for a <see cref="ReflectionTypeLoadException"/> to be thrown 
        /// when calling the <see cref="Assembly.GetTypes"/> method. This extension
        /// method calls that method and catches that exception, returning the types that 
        /// did successfully load from the assembly.
        /// </remarks>
        public static IReadOnlyCollection<Type> GetTypesSafely(this Assembly assembly)
        {
            return
                _assemblyTypes.GetOrAdd(
                    assembly,
                    a =>
                    {
                        try
                        {
                            return a.GetTypes().ToList().AsReadOnly();
                        }
                        catch (ReflectionTypeLoadException ex)
                        {
                            return ex.Types.Where(t => t != null).ToList().AsReadOnly();
                        }
                    });
        }
    }
}
