using System;
using System.Reflection;

namespace Rock.Reflection
{
    public static class IsDefinedInThisAssemblyExtension
    {
        /// <summary>
        /// Determines whether the given <see cref="MemberInfo"/> is defined in the
        /// calling assembly. 
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns>
        /// True, if the member is defined in this assembly. Else false, if the member
        /// is defined in another assembly.
        /// </returns>
        public static bool IsDefinedInThisAssembly(this MemberInfo member)
        {
            var thisAssembly = Assembly.GetCallingAssembly();

            var type = member as Type ?? member.DeclaringType;

            return thisAssembly == type.Assembly;
        }
    }
}
