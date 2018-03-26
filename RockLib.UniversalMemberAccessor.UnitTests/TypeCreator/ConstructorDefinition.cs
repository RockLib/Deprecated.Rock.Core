using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace RockLib.Dynamic.UnitTests.TypeCreator
{
    public class ConstructorDefinition : MemberDefinition
    {
        public ConstructorDefinition(Action<TypeBuilder, List<FieldBuilder>> emitTo)
            : base(emitTo)
        {
        }
    }
}