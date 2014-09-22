using System;
using System.Collections.Generic;
using System.Linq;

namespace Rock.Collections
{
    public class DefaultMemberLocator : DeepEqualityComparer.IMemberLocator
    {
        public virtual IEnumerable<DeepEqualityComparer.PropertyOrField> GetFieldsAndProperties(Type type)
        {
            return
                type.GetProperties()
                    .Where(p =>
                        p.CanRead
                        && p.GetGetMethod() != null
                        && p.GetGetMethod().IsPublic
                        && !p.GetGetMethod().IsStatic)
                    .Select(p => new DeepEqualityComparer.PropertyOrField(p))
                    .Concat(
                        type.GetFields()
                            .Where(f => !f.IsStatic)
                            .Select(f => new DeepEqualityComparer.PropertyOrField(f)))
                    .ToList();
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj.GetType() == typeof(DefaultMemberLocator);
        }

        public override int GetHashCode()
        {
            return typeof(DefaultMemberLocator).GetHashCode();
        }
    }
}