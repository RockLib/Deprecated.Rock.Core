using System.Collections;
using System.Collections.Generic;

namespace Rock.Collections
{
    public sealed class DeepEqualityComparer<T> : IEqualityComparer<T>
    {
        private readonly IEqualityComparer _equalityComparer;

        public DeepEqualityComparer()
        {
            _equalityComparer = new DeepEqualityComparer();
        }

        public DeepEqualityComparer(DeepEqualityComparer.IConfiguration configuration)
        {
            _equalityComparer = new DeepEqualityComparer(configuration);
        }

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
