using System;

namespace RockLib.Dynamic
{
    /// <summary>
    /// A class to provide the <see cref="Unlock(object)"/> and <see cref="Unlock(Type)"/> extension methods.
    /// </summary>
    public static class UnlockExtension
    {
        /// <summary>
        /// Gets a dynamic proxy object (specifically, an instance of <see cref="UniversalMemberAccessor"/>)
        /// for the given object. Returns null if <paramref name="instance"/> is null.
        /// </summary>
        /// <remarks>
        /// If this method is called more than once with the same object, then the value returned
        /// is the same instance of <see cref="UniversalMemberAccessor"/> each time.
        /// <para>This method returns the result of a call to <see cref="UniversalMemberAccessor.Get(object)"/>,
        /// passing the <paramref name="instance"/> parameter.</para>
        /// </remarks>
        /// <param name="instance">An object.</param>
        /// <returns>
        /// A dynamic proxy object enabling access to all members of the given instance, or null
        /// if <paramref name="instance"/> is null.
        /// </returns>
        public static dynamic Unlock(this object instance)
        {
            return UniversalMemberAccessor.Get(instance);
        }

        /// <summary>
        /// Gets a dynamic proxy object (specifically, an instance of <see cref="UniversalMemberAccessor"/>)
        /// for the static members of the given type.
        /// </summary>
        /// <param name="type">The type whose static members will be exposed by the resulting dynamic proxy object.</param>
        /// <returns>A dynamic proxy object enabling access to all static members of the given type.</returns>
        public static dynamic Unlock(this Type type)
        {
            return UniversalMemberAccessor.GetStatic(type);
        }
    }
}