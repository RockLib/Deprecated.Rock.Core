using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Rock.DependencyInjection;

namespace Rock.Serialization
{
    internal class XmlDeserializationProxyEngine<TTarget>
    {
        // ReSharper disable once StaticFieldInGenericType
        private static readonly Regex _enumFlagsRegex = new Regex(@"(?:\s*\|\s*)|(?:\s+or\s+)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private readonly object _proxyInstance;
        private readonly Lazy<Type> _type;
        private readonly Type _baseClassType;
        private XmlAttribute[] _additionalXmlAttributes = new XmlAttribute[0];
        private XmlElement[] _additionalXmlElements = new XmlElement[0];
        private readonly List<XAttribute> _additionalXAttributes = new List<XAttribute>();
        private readonly List<XElement> _additionalXElements = new List<XElement>();

        public XmlDeserializationProxyEngine(
            object proxyInstance, Type defaultType, Type baseClassType)
        {
            if (defaultType == null && !typeof(TTarget).IsAbstract)
            {
                defaultType = typeof(TTarget);
            }
            else
            {
                ThrowIfNotAssignableToT(defaultType);
            }

            _proxyInstance = proxyInstance;
            _type = new Lazy<Type>(() => Type.GetType(TypeAssemblyQualifiedName, true));
            _baseClassType = baseClassType;
            TypeAssemblyQualifiedName =
                defaultType != null
                    ? defaultType.AssemblyQualifiedName
                    : null;
        }

        public string TypeAssemblyQualifiedName { get; set; }

        public XmlAttribute[] AdditionalXmlAttributes
        {
            get { return _additionalXmlAttributes; }
            set { _additionalXmlAttributes = value ?? new XmlAttribute[0]; }
        }

        public XmlElement[] AdditionalXmlElements
        {
            get { return _additionalXmlElements; }
            set { _additionalXmlElements = value ?? new XmlElement[0]; }
        }

        public List<XAttribute> AdditionalXAttributes
        {
            get { return _additionalXAttributes; }
        }

        public List<XElement> AdditionalXElements
        {
            get { return _additionalXElements; }
        }

        public IEnumerable<XObject> AllAdditionalNodes
        {
            get
            {
                return AdditionalXAttributes.Cast<XObject>()
                    .Concat(AdditionalXmlAttributes.Select(x => new XAttribute(XName.Get(x.LocalName, x.NamespaceURI), x.Value)))
                    .Concat(AdditionalXElements)
                    .Concat(AdditionalXmlElements.Select(x => XElement.Load(x.CreateNavigator().ReadSubtree())));
            }
        }

        public TTarget CreateInstance(IResolver resolver)
        {
            Type type;

            try
            {
                type = _type.Value;
            }
            catch (Exception ex)
            {
                if (TypeAssemblyQualifiedName == null)
                {
                    throw new XmlDeserializationProxyException("The required 'type' attribute was not provided.", ex);
                }

                throw new XmlDeserializationProxyException(string.Format(
                    "The value provided for the required 'type' attribute, '{0}', is not a valid assembly-qualified type name.",
                    TypeAssemblyQualifiedName), ex);
            }

            var creationScenario = GetCreationScenario(type, resolver);
            var args = CreateArgs(creationScenario.Parameters, resolver);
            var instance = creationScenario.Constructor.Invoke(args);

            foreach (var property in creationScenario.Properties)
            {
                object value;

                if (TryGetValueForInstance(property.Name, property.PropertyType, out value))
                {
                    property.SetValue(instance, value, null);
                }
            }

            return (TTarget)instance;
        }

        private static void ThrowIfNotAssignableToT(Type type)
        {
            if (type != null
                && !typeof(TTarget).IsAssignableFrom(type))
            {
                throw new ArgumentException(string.Format(
                    "The provided Type, {0}, must be assignable to Type {1}.",
                    type, typeof(TTarget)));
            }
        }

        private CreationScenario GetCreationScenario(Type type, IResolver resolver)
        {
            return new CreationScenario(GetConstructor(type, resolver), type);
        }

        private ConstructorInfo GetConstructor(Type type, IResolver resolver)
        {
            // The constructor with the most resolvable parameters wins.
            // If tied, the constructor with fewest parameters wins.

            return
                (from ctor in type.GetConstructors()
                 let parameters = ctor.GetParameters()
                 orderby
                         parameters.Count(p => CanResolveParameterValue(p, resolver)) descending,
                         parameters.Length ascending
                 select ctor).First();
        }

        /// <summary>
        /// Returns true if a value for the parameter exists in the xml.
        /// </summary>
        private bool CanResolveParameterValue(ParameterInfo parameter, IResolver resolver)
        {
            object dummy;
            if (TryGetValueForInstance(parameter.Name, parameter.ParameterType, out dummy))
            {
                return true;
            }

            if (resolver != null && resolver.CanGet(parameter.ParameterType))
            {
                return true;
            }

            return false;
        }

        private object[] CreateArgs(IEnumerable<ParameterInfo> parameters, IResolver resolver)
        {
            var argsList = new List<object>();

            foreach (var parameter in parameters)
            {
                object argValue;

                if (TryGetValueForInstance(parameter.Name, parameter.ParameterType, out argValue))
                {
                    argsList.Add(argValue);
                }
                else if (resolver != null && resolver.CanGet(parameter.ParameterType))
                {
                    argsList.Add(resolver.Get(parameter.ParameterType));
                }
                else
                {
                    var hasDefaultValue = (parameter.Attributes & ParameterAttributes.HasDefault) ==
                                          ParameterAttributes.HasDefault;

                    argsList.Add(hasDefaultValue ? parameter.DefaultValue : null);
                }
            }

            return argsList.ToArray();
        }

        private bool TryGetValueForInstance(string name, Type type, out object value)
        {
            if (TryGetProxyPropertyValue(name, type, out value))
            {
                if (value == null)
                {
                    object additionalValue;
                    if (TryGetAdditionalValue(name, type, out additionalValue))
                    {
                        value = additionalValue;
                    }
                }

                return true;
            }

            if (TryGetAdditionalValue(name, type, out value))
            {
                return true;
            }

            return false;
        }

        private bool TryGetProxyPropertyValue(string name, Type type, out object value)
        {
            var properties =
                _proxyInstance.GetType().GetProperties()
                    .Where(
                        p =>
                            (_baseClassType == null || p.DeclaringType != _baseClassType) &&
                            p.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && p.PropertyType == type)
                    .OrderBy(p => p.Name, new CaseSensitiveEqualityFirstAsComparedTo(name));

            foreach (var property in properties)
            {
                try
                {
                    value = property.GetValue(_proxyInstance, null);

                    if (value != null)
                    {
                        return true;
                    }
                } // ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                }
            }

            value = null;
            return false;
        }

        private bool TryGetAdditionalValue(string name, Type type, out object value)
        {
            foreach (var additionalNode in GetAdditionalNodes(name))
            {
                var additionalElement = additionalNode as XElement;

                if (additionalElement != null)
                {
                    if (TryGetElementValue(additionalElement, type, out value))
                    {
                        return true;
                    }
                }
                else
                {
                    var additionalAttribute = (XAttribute)additionalNode;

                    if (TryConvert(additionalAttribute.Value, type, out value))
                    {
                        return true;
                    }
                }
            }

            value = null;
            return false;
        }

        private IEnumerable<XObject> GetAdditionalNodes(string name)
        {
            var additionalNodes = AllAdditionalNodes
                .Where(x => GetName(x).Equals(name, StringComparison.OrdinalIgnoreCase))
                .OrderBy(GetName, new CaseSensitiveEqualityFirstAsComparedTo(name))
                .ThenBy(x => x, new ElementsBeforeAttributes());

            return additionalNodes;
        }

        private static string GetName(XObject xObject)
        {
            var xElement = xObject as XElement;
            if (xElement != null)
            {
                return xElement.Name.ToString();
            }

            var xAttribute = xObject as XAttribute;
            if (xAttribute != null)
            {
                return xAttribute.Name.ToString();
            }

            return null;
        }

        private static bool TryGetElementValue(XElement additionalElement, Type type, out object value)
        {
            if (!additionalElement.HasAttributes
                && (!additionalElement.Nodes().Any()
                    || additionalElement.Nodes().All(node => node.NodeType != XmlNodeType.Element)))
            {
                var reader = additionalElement.CreateReader();
                reader.MoveToContent();
                var innerXml = reader.ReadInnerXml();

                if (TryConvert(innerXml, type, out value))
                {
                    return true;
                }
            }

            using (var reader = new StringReader(additionalElement.ToString()))
            {
                try
                {
                    XmlSerializer serializer = null;

                    var typeAttribute = additionalElement.Attribute("type");

                    if (typeAttribute != null)
                    {
                        var typeName = typeAttribute.Value;
                        var typeFromAttribute = Type.GetType(typeName);

                        if (typeFromAttribute != null)
                        {
                            serializer = new XmlSerializer(typeFromAttribute,
                                new XmlRootAttribute(additionalElement.Name.ToString()));
                        }
                    }

                    if (serializer == null)
                    {
                        if (type.IsInterface || type.IsAbstract)
                        {
                            value = null;
                            return false;
                        }

                        serializer = new XmlSerializer(type, new XmlRootAttribute(additionalElement.Name.ToString()));
                    }

                    value = serializer.Deserialize(reader);
                    return true;
                }
                catch
                {
                    value = null;
                    return false;
                }
            }
        }

        private static bool TryConvert(string stringValue, Type type, out object value)
        {
            if (type == typeof(string))
            {
                value = stringValue;
                return true;
            }

            var converter = TypeDescriptor.GetConverter(type);

            if (converter.CanConvertFrom(typeof(string)))
            {
                if (type.IsEnum)
                {
                    stringValue = _enumFlagsRegex.Replace(stringValue, ",");
                }

                value = converter.ConvertFrom(stringValue);
                return true;
            }

            value = null;
            return false;
        }

        private class CreationScenario
        {
            private readonly ConstructorInfo _ctor;
            private readonly ParameterInfo[] _parameters;
            private readonly IEnumerable<PropertyInfo> _properties;

            public CreationScenario(ConstructorInfo ctor, Type type)
            {
                _ctor = ctor;
                _parameters = ctor.GetParameters();

                var parameterNames = _parameters.Select(p => p.Name).ToList();

                _properties =
                    type.GetProperties()
                        .Where(p =>
                            p.CanRead
                            && p.CanWrite
                            && p.GetGetMethod(true).IsPublic
                            && p.GetSetMethod(true).IsPublic
                            &&
                            parameterNames.All(
                                parameterName => !parameterName.Equals(p.Name, StringComparison.OrdinalIgnoreCase)));
            }

            public ConstructorInfo Constructor
            {
                get { return _ctor; }
            }

            public IEnumerable<ParameterInfo> Parameters
            {
                get { return _parameters; }
            }

            public IEnumerable<PropertyInfo> Properties
            {
                get { return _properties; }
            }
        }

        private class CaseSensitiveEqualityFirstAsComparedTo : IComparer<string>
        {
            private readonly string _nameToMatch;

            public CaseSensitiveEqualityFirstAsComparedTo(string nameToMatch)
            {
                _nameToMatch = nameToMatch;
            }

            public int Compare(string lhs, string rhs)
            {
                if (string.Equals(lhs, rhs, StringComparison.Ordinal))
                {
                    return 0;
                }

                if (string.Equals(lhs, _nameToMatch, StringComparison.Ordinal))
                {
                    return -1;
                }

                if (string.Equals(rhs, _nameToMatch, StringComparison.Ordinal))
                {
                    return 1;
                }

                return 0;
            }
        }

        private class ElementsBeforeAttributes : IComparer<XObject>
        {
            public int Compare(XObject lhs, XObject rhs)
            {
                if (lhs is XElement)
                {
                    return (rhs is XAttribute) ? -1 : 0;
                }

                return (rhs is XElement) ? 1 : 0;
            }
        }
    }
}