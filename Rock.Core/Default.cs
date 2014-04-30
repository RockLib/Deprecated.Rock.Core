using System;
using System.Dynamic;
using Rock.Conversion;
using Rock.DependencyInjection.Heuristics;
using Rock.Serialization;

namespace Rock
{
    public static class Default
    {
        private static readonly Lazy<IJsonSerializer> _defaultJsonSerializer;
        private static Lazy<IJsonSerializer> _jsonSerializer;

        private static readonly Lazy<IConvertTo<ExpandoObject>> _defaultConvertToExpandoObject;
        private static Lazy<IConvertTo<ExpandoObject>> _convertToExpandoObject;

        private static readonly Lazy<IConstructorSelector> _defaultConstructorSelector;
        private static Lazy<IConstructorSelector> _constructorSelector;

        static Default()
        {
            _defaultJsonSerializer = new Lazy<IJsonSerializer>(() => new NewtonsoftJsonSerializer());
            _jsonSerializer = _defaultJsonSerializer;

            _defaultConvertToExpandoObject = 
                new Lazy<IConvertTo<ExpandoObject>>(() => new ReflectinatorConvertToExpandoObject());
            _convertToExpandoObject = _defaultConvertToExpandoObject;
        
            _defaultConstructorSelector = new Lazy<IConstructorSelector>(() => new ConstructorSelector());
            _constructorSelector = _defaultConstructorSelector;
        }

        public static IJsonSerializer JsonSerializer
        {
            get { return _jsonSerializer.Value; }
            set
            {
                if (value == null)
                {
                    _jsonSerializer = _defaultJsonSerializer;
                }
                else if (!CurrentJsonSerializerIsSameAs(value))
                {
                    _jsonSerializer = new Lazy<IJsonSerializer>(() => value);
                }
            }
        }

        private static bool CurrentJsonSerializerIsSameAs(IJsonSerializer value)
        {
            return _jsonSerializer.IsValueCreated && ReferenceEquals(_jsonSerializer.Value, value);
        }

        public static IConvertTo<ExpandoObject> ConvertToExpandoObject
        {
            get { return _convertToExpandoObject.Value; }
            set
            {
                if (value == null)
                {
                    _convertToExpandoObject = _defaultConvertToExpandoObject;
                }
                else if (!CurrentConvertToExpandoObjectIsSameAs(value))
                {
                    _convertToExpandoObject = new Lazy<IConvertTo<ExpandoObject>>(() => value);
                }
            }
        }

        private static bool CurrentConvertToExpandoObjectIsSameAs(IConvertTo<ExpandoObject> value)
        {
            return _convertToExpandoObject.IsValueCreated && ReferenceEquals(_convertToExpandoObject.Value, value);
        }

        public static IConstructorSelector ConstructorSelector
        {
            get { return _constructorSelector.Value; }
            set
            {
                if (value == null)
                {
                    _constructorSelector = _defaultConstructorSelector;
                }
                else if (!CurrentConstructorSelectorIsSameAs(value))
                {
                    _constructorSelector = new Lazy<IConstructorSelector>(() => value);
                }
            }
        }

        private static bool CurrentConstructorSelectorIsSameAs(IConstructorSelector value)
        {
            return _constructorSelector.IsValueCreated && ReferenceEquals(_constructorSelector.Value, value);
        }
    }
}
