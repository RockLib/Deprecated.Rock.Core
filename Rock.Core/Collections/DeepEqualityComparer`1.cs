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

        public bool Equals(T lhs, T rhs)
        {
            return _equalityComparer.Equals(lhs, rhs);
        }

        public int GetHashCode(T instance)
        {
            return _equalityComparer.GetHashCode(instance);
        }
    }
}
