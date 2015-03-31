using System.Reflection;

namespace Rock.Reflection
{
    public static class IsPublicExtension
    {
        public static bool IsPublic(this PropertyInfo propertyInfo)
        {
            return propertyInfo.HasPublicGetter() || propertyInfo.HasPublicSetter();
        }

        public static bool HasPublicGetter(this PropertyInfo propertyInfo)
        {
            return propertyInfo.CanRead && propertyInfo.GetGetMethod(false) != null;
        }

        public static bool HasPublicSetter(this PropertyInfo propertyInfo)
        {
            return propertyInfo.CanWrite && propertyInfo.GetSetMethod(false) != null;
        }
    }
}
