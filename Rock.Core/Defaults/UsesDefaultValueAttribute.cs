using System;

namespace Rock.Defaults
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class UsesDefaultValueAttribute : Attribute
    {
        private readonly Type _defaultType;
        private readonly string _propertyName;

        public UsesDefaultValueAttribute(Type defaultType, string propertyName)
        {
            _defaultType = defaultType;
            _propertyName = propertyName;
        }

        public string PropertyName
        {
            get { return _propertyName; }
        }

        public Type DefaultType
        {
            get { return _defaultType; }
        }
    }
}