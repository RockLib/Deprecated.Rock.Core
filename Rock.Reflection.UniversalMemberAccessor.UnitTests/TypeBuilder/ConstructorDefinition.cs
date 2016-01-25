using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Rock.Reflection.UnitTests.TypeBuilder
{
    public class ConstructorDefinition : MemberDefinition
    {
        public ConstructorDefinition(Action<System.Reflection.Emit.TypeBuilder, List<FieldBuilder>> emitTo)
            : base(emitTo)
        {
        }
    }
}