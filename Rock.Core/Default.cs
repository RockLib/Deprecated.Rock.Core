using System;
using System.Dynamic;
using Rock.Conversion;
using Rock.DependencyInjection.Heuristics;
using Rock.Net;
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

        private static readonly Lazy<IEndpointDetector> _defaultEndpointDetector;
        private static Lazy<IEndpointDetector> _endpointDetector;

        private static readonly Lazy<IEndpointSelector> _defaultEndpointSelector;
        private static Lazy<IEndpointSelector> _endpointSelector;

        static Default()
        {
            _defaultJsonSerializer = new Lazy<IJsonSerializer>(() => new NewtonsoftJsonSerializer());
            _jsonSerializer = _defaultJsonSerializer;

            _defaultConvertToExpandoObject = 
                new Lazy<IConvertTo<ExpandoObject>>(() => new ReflectinatorConvertToExpandoObject());
            _convertToExpandoObject = _defaultConvertToExpandoObject;
        
            _defaultConstructorSelector = new Lazy<IConstructorSelector>(() => new ConstructorSelector());
            _constructorSelector = _defaultConstructorSelector;

            _defaultEndpointDetector = new Lazy<IEndpointDetector>(() => new EndpointDetector());
            _endpointDetector = _defaultEndpointDetector;

            _defaultEndpointSelector = new Lazy<IEndpointSelector>(() => new EndpointSelector(_defaultEndpointDetector.Value));
            _endpointSelector = _defaultEndpointSelector;
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

        public static IEndpointDetector EndpointDetector
        {
            get { return _endpointDetector.Value; }
            set
            {
                if (value == null)
                {
                    _endpointDetector = _defaultEndpointDetector;
                }
                else if (!CurrentEndpointDetectorIsSameAs(value))
                {
                    _endpointDetector = new Lazy<IEndpointDetector>(() => value);
                }
            }
        }

        private static bool CurrentEndpointDetectorIsSameAs(IEndpointDetector value)
        {
            return _endpointDetector.IsValueCreated && ReferenceEquals(_endpointDetector.Value, value);
        }

        public static IEndpointSelector EndpointSelector
        {
            get { return _endpointSelector.Value; }
            set
            {
                if (value == null)
                {
                    _endpointSelector = _defaultEndpointSelector;
                }
                else if (!CurrentEndpointSelectorIsSameAs(value))
                {
                    _endpointSelector = new Lazy<IEndpointSelector>(() => value);
                }
            }
        }

        private static bool CurrentEndpointSelectorIsSameAs(IEndpointSelector value)
        {
            return _endpointSelector.IsValueCreated && ReferenceEquals(_endpointSelector.Value, value);
        }
    }
}
