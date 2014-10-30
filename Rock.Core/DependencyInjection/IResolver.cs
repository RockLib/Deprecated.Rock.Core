using System;

namespace Rock.DependencyInjection
{
    /// <summary>
    /// Defines methods for obtaining instances of arbitrary types.
    /// </summary>
    public interface IResolver
    {
        /// <summary>
        /// Returns whether this instance of <see cref="IResolver"/> can get an instance of the specified
        /// type.
        /// </summary>
        /// <param name="type">The type to determine whether this instance is able to get an instance of.</param>
        /// <returns>True, if this instance can get an instance of the specified type. False, otherwise.</returns>
        bool CanGet(Type type);

        /// <summary>
        /// Gets an instance of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of object to return.</typeparam>
        /// <returns>An instance of type <typeparamref name="T"/>.</returns>
        T Get<T>();

        /// <summary>
        /// Gets an instance of type <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type of object to return.</param>
        /// <returns>An instance of type <paramref name="type"/></returns>
        object Get(Type type);

        /// <summary>
        /// Returns a new instance of <see cref="IResolver"/> that is the result of a merge operation between
        /// this instance of <see cref="IResolver"/> and <paramref name="secondaryResolver"/>.
        /// </summary>
        /// <param name="secondaryResolver">A secondary <see cref="IResolver"/>.</param>
        /// <returns>An instance of <see cref="IResolver"/> resulting from the merge operation.</returns>
        /// <remarks>
        /// Implementors of this method should return an instance of <see cref="IResolver"/> that attempts to
        /// resolve via this instance of <see cref="IResolver"/> before attempting to resolve from
        /// <paramref name="secondaryResolver"/>, recursively. It should set up the relationship such that
        /// the resulting <see cref="IResolver"/> has access to <paramref name="secondaryResolver"/>, but not
        /// the other way around - <paramref name="secondaryResolver"/> shouldn't "know" about this instance 
        /// of <see cref="IResolver"/> or the <see cref="IResolver"/> that is returned by this method.
        /// </remarks>
        IResolver MergeWith(IResolver secondaryResolver);
    }
}