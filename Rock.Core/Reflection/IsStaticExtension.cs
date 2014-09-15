using System.Reflection;

namespace Rock.Reflection
{
    public static class IsStaticExtension
    {
        public static bool IsStatic(this PropertyInfo propertyInfo)
        {
            return (propertyInfo.CanRead && propertyInfo.GetGetMethod(true).IsStatic)
                   || (propertyInfo.CanWrite && propertyInfo.GetSetMethod(true).IsStatic);
        }
    }
}
