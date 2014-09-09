using System.Collections;
using System.Collections.Generic;

namespace Rock.Collections
{
    public sealed class DeepEqualityComparer<T> : IEqualityComparer<T>
    {
        private readonly IEqualityComparer _equalityComparer = new DeepEqualityComparer();

        bool IEqualityComparer<T>.Equals(T lhs, T rhs)
        {
            return _equalityComparer.Equals(lhs, rhs);
        }

        int IEqualityComparer<T>.GetHashCode(T instance)
        {
            return _equalityComparer.GetHashCode(instance);
        }
    }
}
