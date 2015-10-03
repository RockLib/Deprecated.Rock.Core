namespace Rock.Reflection
{
    /// <summary>
    /// A class to provide the <see cref="Unlock"/> extension method.
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
    }
}