using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rock.Utilities
{
    public static class AttributeLocator
    {
        public static IEnumerable<MemberInfo> FindMembersDecoratedWith<TAttribute>(Assembly assembly)
            where TAttribute : Attribute
        {
            var attributeType = typeof(TAttribute);

            foreach (var type in assembly.GetTypes())
            {
                if (Attribute.IsDefined(type, attributeType))
                {
                    yield return type;
                }

                var members =
                    type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                        .Where(m => !(m is Type) && Attribute.IsDefined(m, attributeType));

                foreach (var member in members)
                {
                    yield return member;
                }
            }
        }
    }
}
