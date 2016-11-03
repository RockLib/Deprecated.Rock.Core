using System;

namespace Rock.Reflection.Emit
{
    public class Parameter
    {
        private static readonly object _notADefaultValue = new object();

        private readonly string _name;
        private readonly Type _type;
        private readonly string _fieldToSet;
        private readonly object _defaultValue;

        public Parameter(Type type, string name = null, string fieldToSet = null)
            : this(_notADefaultValue, type, name, fieldToSet)
        {
        }
        
        public Parameter(object defaultValue, Type type, string name = null, string fieldToSet = null)
        {
            if (defaultValue != _notADefaultValue)
            {
                if (type.IsValueType)
                {
                    if (defaultValue == null)
                    {
                        throw new ArgumentException("defaultValue cannot be null when 'type' is a value type.", "defaultValue");
                    }

                    if (type != defaultValue.GetType())
                    {
                        throw new ArgumentException("defaultValue must be the same type as 'type' when 'type' is a value type.", "defaultValue");
                    }
                }
                else
                {
                    if (defaultValue != null)
                    {
                        throw new ArgumentException("defaultValue must be null when 'type' is a reference type.", "defaultValue");
                    }
                }
            }

            _defaultValue = defaultValue;
            _name = name;
            _type = type;
            _fieldToSet = fieldToSet;
        }

        public string Name { get { return _name; } }
        public Type Type { get { return _type; } }
        public string FieldToSet { get { return _fieldToSet; } }
        public object DefaultValue { get { return _defaultValue; } }
        public bool HasDefaultValue { get { return _defaultValue != _notADefaultValue; } }

        public static implicit operator Parameter(Type type)
        {
            return new Parameter(type);
        }
    }
}