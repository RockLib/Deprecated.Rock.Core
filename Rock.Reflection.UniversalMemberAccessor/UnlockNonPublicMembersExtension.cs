namespace Rock.Reflection
{
    /// <summary>
    /// A class to provide the <see cref="UnlockNonPublicMembers"/> extension method.
    /// </summary>
    public static class UnlockNonPublicMembersExtension
    {
        /// <summary>
        /// Gets a dynamic proxy object (specifically, an instance of <see cref="UniversalMemberAccessor"/>)
        /// for the object.
        /// </summary>
        /// <remarks>
        /// If this method is called more than once with the same object, then the value returned
        /// is the same instance of <see cref="UniversalMemberAccessor"/> each time.
        /// </remarks>
        /// <param name="instance">An object.</param>
        /// <returns>A dynamic proxy object enabling access to all members of the provided object.</returns>
        public static dynamic UnlockNonPublicMembers(this object instance)
        {
            return UniversalMemberAccessor.Get(instance);
        }
    }
}