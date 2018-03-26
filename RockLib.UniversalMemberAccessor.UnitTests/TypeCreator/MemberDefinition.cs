using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace RockLib.Dynamic.UnitTests.TypeCreator
{
    public class MemberDefinition
    {
        private readonly Action<TypeBuilder, List<FieldBuilder>> _emitTo;

        public MemberDefinition(Action<TypeBuilder, List<FieldBuilder>> emitTo)
        {
            _emitTo = emitTo;
        }

        public void EmitTo(TypeBuilder typeBuilder, List<FieldBuilder> declaredFields)
        {
            _emitTo(typeBuilder, declaredFields);
        }
    }
}