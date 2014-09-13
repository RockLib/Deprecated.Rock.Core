using System;

namespace Rock.Reflection
{
    public static class IsLessSpecificThanExtension
    {
        public static bool IsLessSpecificThan(this Type thisType, Type comparisonType)
        {
            return thisType != comparisonType && thisType.IsAssignableFrom(comparisonType);
        }
    }
}
