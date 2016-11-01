using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Rock.Reflection.Emit
{
    public class ConstructorDefinition : MemberDefinition
    {
        public ConstructorDefinition(Action<TypeBuilder, List<FieldBuilder>> emitTo)
            : base(emitTo)
        {
        }
    }
}