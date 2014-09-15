using System.Reflection;

namespace Rock.Reflection
{
    public static class IsPublicExtension
    {
        public static bool IsPublic(this PropertyInfo propertyInfo)
        {
            return (propertyInfo.CanRead && propertyInfo.HasPublicGetMethod())
                   || (propertyInfo.CanWrite && propertyInfo.HasPublicSetMethod());
        }

        private static bool HasPublicGetMethod(this PropertyInfo propertyInfo)
        {
            var getMethod = propertyInfo.GetGetMethod();

            return getMethod != null && getMethod.IsPublic;
        }

        private static bool HasPublicSetMethod(this PropertyInfo propertyInfo)
        {
            var setMethod = propertyInfo.GetSetMethod();

            return setMethod != null && setMethod.IsPublic;
        }
    }
}
