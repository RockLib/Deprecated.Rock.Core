using System;

namespace Rock.Reflection.UnitTests.TypeBuilder
{
    public class Parameter
    {
        private readonly Type _type;
        private readonly string _fieldToSet;
        
        public Parameter(Type type, string fieldToSet)
        {
            _type = type;
            _fieldToSet = fieldToSet;
        }

        public Type Type { get { return _type; } }
        public string FieldToSet { get { return _fieldToSet; } }

        public static implicit operator Parameter(Type type)
        {
            return new Parameter(type, null);
        }
    }
}