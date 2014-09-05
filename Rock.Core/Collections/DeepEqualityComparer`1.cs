using System.Collections.Generic;

namespace Rock.Collections
{
    public sealed class DeepEqualityComparer<T> : IEqualityComparer<T>
    {
        private static readonly IEqualityComparer<T> _instance = new DeepEqualityComparer<T>();

        private DeepEqualityComparer()
        {
        }

        public static IEqualityComparer<T> Instance
        {
            get { return _instance; }
        }

        bool IEqualityComparer<T>.Equals(T lhs, T rhs)
        {
            return DeepEqualityComparer.Instance.Equals(lhs, rhs);
        }

        int IEqualityComparer<T>.GetHashCode(T instance)
        {
            return DeepEqualityComparer.Instance.GetHashCode(instance);
        }
    }
}
