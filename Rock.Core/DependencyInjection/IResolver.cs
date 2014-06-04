using System;

namespace Rock.DependencyInjection
{
    public interface IResolver
    {
        bool CanGet(Type type);
        T Get<T>();
        object Get(Type type);

        /// <summary>
        /// Merge this instance of <see cref="IResolver"/> with <paramref name="secondaryResolver"/>. The
        /// resulting <see cref="IResolver"/> should always try to use this <see cref="IResolver"/> before
        /// <paramref name="secondaryResolver"/>, recursively. It should set up the relationship such that
        /// this <see cref="IResolver"/> has access to <paramref name="secondaryResolver"/>, but not the other
        /// way around - <paramref name="secondaryResolver"/> shouldn't know about this instance of
        /// <see cref="IResolver"/> or the <see cref="IResolver"/> that is returned by this method.
        /// </summary>
        /// <param name="secondaryResolver">The secondary <see cref="IResolver"/>.</param>
        /// <returns>
        /// An instance of <see cref="IResolver"/> that tries to use this instance of <see cref="IResolver"/>
        /// first, but, if this instance of <see cref="IResolver"/> is not suitable, then
        /// <paramref name="secondaryResolver"/> is used.
        /// </returns>
        IResolver MergeWith(IResolver secondaryResolver);
    }
}