using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Rock.Reflection.UnitTests.TypeBuilder
{
    public class MemberDefinition
    {
        private readonly Action<System.Reflection.Emit.TypeBuilder, List<FieldBuilder>> _emitTo;

        public MemberDefinition(Action<System.Reflection.Emit.TypeBuilder, List<FieldBuilder>> emitTo)
        {
            _emitTo = emitTo;
        }

        public void EmitTo(System.Reflection.Emit.TypeBuilder typeBuilder, List<FieldBuilder> declaredFields)
        {
            _emitTo(typeBuilder, declaredFields);
        }
    }
}