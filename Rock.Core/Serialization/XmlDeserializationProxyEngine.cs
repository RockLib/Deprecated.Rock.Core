using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Rock.DependencyInjection;
using Rock.Reflection;

namespace Rock.Serialization
{
    internal class XmlDeserializationProxyEngine<TTarget>
    {
        private const BindingFlags PublicInstance = BindingFlags.Public | BindingFlags.Instance;

        // ReSharper disable once StaticFieldInGenericType
        private static readonly Regex _enumFlagsRegex = new Regex(@"(?:\s*\|\s*)|(?:\s+or\s+)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private readonly object _proxyInstance;
        private readonly Lazy<Type> _concreteTargetType;
        private readonly Type _baseClassType;
        private XmlAttribute[] _additionalXmlAttributes = new XmlAttribute[0];
        private XmlElement[] _additionalXmlElements = new XmlElement[0];
        private readonly List<XAttribute> _additionalXAttributes = new List<XAttribute>();
        private readonly List<XElement> _additionalXElements = new List<XElement>();

        private readonly ConcurrentDictionary<Tuple<string, Type>, object> _memberValueCache = new ConcurrentDictionary<Tuple<string, Type>, object>(); 

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
            _concreteTargetType = new Lazy<Type>(() => Type.GetType(TypeAssemblyQualifiedName, true));
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

        public TTarget CreateInstance(IResolver resolver)
        {
            var targetCreationScenario = GetTargetCreationScenario(GetConcreteTargetType(), resolver);
            var args = CreateConstructorArgs(targetCreationScenario.Parameters, resolver);
            var targetInstance = targetCreationScenario.Constructor.Invoke(args);

            foreach (var targetMember in targetCreationScenario.Members)
            {
                SetTargetMemberValue(targetInstance, targetMember);
            }

            _memberValueCache.Clear();

            return (TTarget)targetInstance;
        }

        private void SetTargetMemberValue(object targetInstance, MemberInfo targetMember)
        {
            object targetMemberValue;

            var targetProperty = targetMember as PropertyInfo;
            if (targetProperty != null)
            {
                if (TryGetTargetMemberValue(targetProperty.Name, targetProperty.PropertyType, out targetMemberValue))
                {
                    targetProperty.SetValue(targetInstance, targetMemberValue);
                }
            }
            else
            {
                var targetField = (FieldInfo)targetMember;
                if (TryGetTargetMemberValue(targetField.Name, targetField.FieldType, out targetMemberValue))
                {
                    targetField.SetValue(targetInstance, targetMemberValue);
                }
            }
        }

        private Type GetConcreteTargetType()
        {
            try
            {
                return _concreteTargetType.Value;
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

        private TargetCreationScenario GetTargetCreationScenario(Type concreteTargetType, IResolver resolver)
        {
            return new TargetCreationScenario(GetConstructor(concreteTargetType, resolver), concreteTargetType);
        }

        private ConstructorInfo GetConstructor(Type concreteTargetType, IResolver resolver)
        {
            var comparer = new ConstructorComparer(this, resolver);

            var constructorGroups = concreteTargetType.GetConstructors()
                .GroupBy(c => c, (key, constructors) => new { key, constructors }, comparer)
                .OrderBy(x => x.key, comparer)
                .Select(x => x.constructors.ToArray())
                .ToArray();

            if (constructorGroups.Length == 0)
            {
                throw new InvalidOperationException("No legal constructors found given the provided XML elements and attributes.");
            }

            if (constructorGroups[0].Length > 1)
            {
                throw new InvalidOperationException("Ambiguous constructors found given the provided XML elements and attributes.");
            }

            return constructorGroups[0][0];
        }

        internal class ConstructorComparer : IComparer<ConstructorInfo>, IEqualityComparer<ConstructorInfo>
        {
            private readonly XmlDeserializationProxyEngine<TTarget> _engine;
            private readonly IResolver _resolver;

            public ConstructorComparer(XmlDeserializationProxyEngine<TTarget> engine, IResolver resolver)
            {
                _engine = engine;
                _resolver = resolver;
            }

            public int Compare(ConstructorInfo lhs, ConstructorInfo rhs)
            {
                if (lhs == rhs)
                {
                    return 0;
                }

                Debug.Assert(lhs.DeclaringType == rhs.DeclaringType, "Can only compare constructors for the same type.");

                var lhsParameters = lhs.GetParameters();
                var rhsParameters = rhs.GetParameters();

                var lhsUnresolvableParameters = lhsParameters.Where(p => !_engine.CanResolveParameterValue(p, _resolver)).ToArray();
                var rhsUnresolvableParameters = rhsParameters.Where(p => !_engine.CanResolveParameterValue(p, _resolver)).ToArray();

                var lhsResolvableParameterCount = lhsParameters.Length - lhsUnresolvableParameters.Length;
                var rhsResolvableParameterCount = rhsParameters.Length - rhsUnresolvableParameters.Length;

                var lhsUnresolvableParameterWithDefaultValueCount = lhsUnresolvableParameters.Count(p => p.HasDefaultValue);
                var rhsUnresolvableParameterWithDefaultValueCount = rhsUnresolvableParameters.Count(p => p.HasDefaultValue);

                var lhsUnresolvableParameterWithoutDefaultValueCount = lhsUnresolvableParameters.Length - lhsUnresolvableParameterWithDefaultValueCount;
                var rhsUnresolvableParameterWithoutDefaultValueCount = rhsUnresolvableParameters.Length - rhsUnresolvableParameterWithDefaultValueCount;

                if (lhsUnresolvableParameterWithoutDefaultValueCount < rhsUnresolvableParameterWithoutDefaultValueCount)
                {
                    return -1;
                }
                if (lhsUnresolvableParameterWithoutDefaultValueCount > rhsUnresolvableParameterWithoutDefaultValueCount)
                {
                    return 1;
                }

                if (lhsResolvableParameterCount > rhsResolvableParameterCount)
                {
                    return -1;
                }
                if (lhsResolvableParameterCount < rhsResolvableParameterCount)
                {
                    return 1;
                }

                var commonParameterNames = lhsParameters.Where(lp => rhsParameters.Any(rp => lp.Name == rp.Name)).Select(p => p.Name).OrderBy(n => n).ToArray();
                var lhsCommonParameters = commonParameterNames.Select(n => lhsParameters.Single(p => p.Name == n)).ToArray();
                var rhsCommonParameters = commonParameterNames.Select(n => rhsParameters.Single(p => p.Name == n)).ToArray();

                var score = 0;

                for (int i = 0; i < commonParameterNames.Length; i++)
                {
                    var lhsParameterType = Nullable.GetUnderlyingType(lhsCommonParameters[i].ParameterType) ?? lhsCommonParameters[i].ParameterType;
                    var rhsParameterType = Nullable.GetUnderlyingType(rhsCommonParameters[i].ParameterType) ?? rhsCommonParameters[i].ParameterType;

                    if (lhsParameterType == rhsParameterType)
                    {
                        continue;
                    }

                    if (rhsParameterType.IsLessSpecificThan(lhsParameterType))
                    {
                        score--;
                    }
                    if (lhsParameterType.IsLessSpecificThan(rhsParameterType))
                    {
                        score++;
                    }
                }

                if (score < 0)
                {
                    return -1;
                }
                if (score > 0)
                {
                    return 1;
                }

                if (lhsUnresolvableParameterWithDefaultValueCount < rhsUnresolvableParameterWithDefaultValueCount)
                {
                    return -1;
                }
                if (lhsUnresolvableParameterWithDefaultValueCount > rhsUnresolvableParameterWithDefaultValueCount)
                {
                    return 1;
                }

                //if (lhs.GetConstructorChain().Contains(rhs))
                //{
                //    return -1;
                //}
                //if (rhs.GetConstructorChain().Contains(lhs))
                //{
                //    return 1;
                //}

                return 0;
            }

            public bool Equals(ConstructorInfo lhs, ConstructorInfo rhs)
            {
                return Compare(lhs, rhs) == 0;
            }

            public int GetHashCode(ConstructorInfo constructor)
            {
                return constructor.DeclaringType == null ? 0 : constructor.DeclaringType.GetHashCode();
            }
        }

        /// <summary>
        /// Returns true if a value for the parameter exists in the xml.
        /// </summary>
        private bool CanResolveParameterValue(ParameterInfo targetParameter, IResolver resolver)
        {
            object dummy;
            if (TryGetTargetMemberValue(targetParameter.Name, targetParameter.ParameterType, out dummy))
            {
                return true;
            }

            if (resolver != null && resolver.CanGet(targetParameter.ParameterType))
            {
                return true;
            }

            return false;
        }

        private object[] CreateConstructorArgs(
            IEnumerable<ParameterInfo> targetConstructorParameters, IResolver resolver)
        {
            return targetConstructorParameters.Select(parameter =>
            {
                object argValue;
                if (TryGetTargetMemberValue(parameter.Name, parameter.ParameterType, out argValue))
                    return argValue;
                if (resolver != null && resolver.CanGet(parameter.ParameterType))
                    return resolver.Get(parameter.ParameterType);
                return parameter.HasDefaultValue ? parameter.DefaultValue : null;
            }).ToArray();
        }

        private bool TryGetTargetMemberValue(
            string targetMemberName, Type targetMemberType, out object targetMemberValue)
        {
            targetMemberValue = _memberValueCache.GetOrAdd(
                Tuple.Create(targetMemberName, targetMemberType),
                t =>
                {
                    var memberName = t.Item1;
                    var memberType = t.Item2;

                    var valueFactories =
                        GetMatchingProxyProperties(memberName, memberType)
                            .Concat(GetMatchingProxyFields(memberName, memberType))
                            .Concat(GetMatchingAdditionalAttributes(memberName, memberType))
                            .Concat(GetMatchingAdditionalElements(memberName, memberType))
                            .OrderBy(valueFactory => valueFactory.Name, new CaseSensitiveEqualityFirstAsComparedTo(memberName))
                            .ThenBy(valueFactory => valueFactory.Source); // AdditionalNode first, ProxyMember last.

                    foreach (var valueFactory in valueFactories)
                    {
                        object memberValue;
                        if (valueFactory.TryGetValue(out memberValue))
                        {
                            Debug.Assert(memberValue != null);
                            return memberValue;
                        }
                    }

                    return null;
                });

            return targetMemberValue != null;
        }

        private IEnumerable<ValueFactory> GetMatchingAdditionalAttributes(
            string targetMemberName, Type targetMemberType)
        {
            return GetAllAdditionalAttributes()
                .Where(attribute => AreMatchingNames(attribute.Name.LocalName, targetMemberName))
                .Select(attribute => new ValueFactory(attribute.Name.LocalName, ValueSource.AdditionalNode,
                    (out object value) => TryConvert(attribute.Value, targetMemberType, out value)));
        }

        private IEnumerable<ValueFactory> GetMatchingAdditionalElements(
            string targetMemberName, Type targetMemberType)
        {
            return GetAllAdditionalElements()
                .Where(element => AreMatchingNames(element.Name.LocalName, targetMemberName))
                .Select(element => new ValueFactory(element.Name.LocalName, ValueSource.AdditionalNode,
                    (out object value) => TryGetElementValue(element, targetMemberType, out value)));
        }

        private IEnumerable<XAttribute> GetAllAdditionalAttributes()
        {
            return AdditionalXAttributes.Concat(
                AdditionalXmlAttributes.Select(x =>
                    new XAttribute(XName.Get(x.LocalName, x.NamespaceURI), x.Value)));
        }

        private IEnumerable<XElement> GetAllAdditionalElements()
        {
            return AdditionalXElements.Concat(
                AdditionalXmlElements.Select(x =>
                    XElement.Load(x.CreateNavigator().ReadSubtree())));
        }

        private IEnumerable<ValueFactory> GetMatchingProxyProperties(
            string targetMemberName, Type targetMemberType)
        {
            return GetMatchingProxyMembers(targetMemberName, targetMemberType,
                type => type.GetProperties(PublicInstance).Where(p => p.CanRead && p.GetGetMethod(true).IsPublic),
                property => property.PropertyType,
                property => property.GetValue(_proxyInstance));
        }

        private IEnumerable<ValueFactory> GetMatchingProxyFields(
            string targetMemberName, Type targetMemberType)
        {
            return GetMatchingProxyMembers(targetMemberName, targetMemberType,
                type => type.GetFields(PublicInstance),
                field => field.FieldType,
                field => field.GetValue(_proxyInstance));
        }

        private IEnumerable<ValueFactory> GetMatchingProxyMembers<TMemberInfo>(
            string targetMemberName, Type targetMemberType,
            Func<Type, IEnumerable<TMemberInfo>> getAllProxyMembers,
            Func<TMemberInfo, Type> getProxyMemberType,
            Func<TMemberInfo, object> getProxyMemberValue)
            where TMemberInfo : MemberInfo
        {
            return getAllProxyMembers(_proxyInstance.GetType())
                .Where(proxyMember =>
                    (_baseClassType == null || getProxyMemberType(proxyMember) != _baseClassType)
                    && AreMatchingNames(proxyMember.Name, targetMemberName)
                    && getProxyMemberType(proxyMember) == targetMemberType)
                    .Select(m => new ValueFactory(m.Name, ValueSource.ProxyMember,
                        () => getProxyMemberValue(m)));
        }

        private static bool AreMatchingNames(string lhs, string rhs)
        {
            return lhs == rhs
                || (lhs.Length == rhs.Length
                    && char.ToLower(lhs[0]) == char.ToLower(rhs[0])
                    && lhs.Substring(1, lhs.Length - 1) == rhs.Substring(1, rhs.Length - 1));
        }

        private static bool TryGetElementValue(
            XElement additionalElement, Type targetMemberType, out object value)
        {
            if (!additionalElement.HasAttributes
                && (!additionalElement.Nodes().Any()
                    || additionalElement.Nodes().All(node => node.NodeType != XmlNodeType.Element)))
            {
                var reader = additionalElement.CreateReader();
                reader.MoveToContent();
                var innerXml = reader.ReadInnerXml();

                if (TryConvert(innerXml, targetMemberType, out value))
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
                                new XmlRootAttribute(additionalElement.Name.LocalName));
                        }
                    }

                    if (serializer == null)
                    {
                        if (targetMemberType.IsInterface || targetMemberType.IsAbstract)
                        {
                            value = null;
                            return false;
                        }

                        serializer = new XmlSerializer(targetMemberType,
                            new XmlRootAttribute(additionalElement.Name.LocalName));
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

        private class TargetCreationScenario
        {
            private readonly ConstructorInfo _constructor;
            private readonly ParameterInfo[] _parameters;
            private readonly IEnumerable<MemberInfo> _members;

            public TargetCreationScenario(ConstructorInfo constructor, Type concreteTargetType)
            {
                _constructor = constructor;
                _parameters = constructor.GetParameters();

                Func<MemberInfo, bool> memberNameDoesNotMatchAnyParameterNames = member =>
                    !_parameters.Any(parameter => AreMatchingNames(member.Name, parameter.Name));

                _members =
                    concreteTargetType.GetProperties(PublicInstance)
                        .Where(p => p.CanRead && p.CanWrite && p.GetGetMethod(true).IsPublic && p.GetSetMethod(true).IsPublic)
                    .Concat(concreteTargetType.GetFields(PublicInstance)
                        .Where(f => !f.IsInitOnly).Cast<MemberInfo>())
                    .Where(memberNameDoesNotMatchAnyParameterNames);
            }

            public ConstructorInfo Constructor { get { return _constructor; } }
            public IEnumerable<ParameterInfo> Parameters { get { return _parameters; } }
            public IEnumerable<MemberInfo> Members { get { return _members; } }
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
                if (string.Equals(lhs, rhs, StringComparison.Ordinal)) return 0;
                if (string.Equals(lhs, _nameToMatch, StringComparison.Ordinal)) return -1;
                if (string.Equals(rhs, _nameToMatch, StringComparison.Ordinal)) return 1;
                return 0;
            }
        }

        private class ValueFactory
        {
            private readonly string _name;
            private readonly ValueSource _source;
            private readonly TryFunc<object> _tryGetValue;

            public ValueFactory(string name, ValueSource source, Func<object> getValue)
                : this(name, source, Try(getValue))
            {
            }

            public ValueFactory(string name, ValueSource source, TryFunc<object> tryGetValue)
            {
                _name = name;
                _source = source;
                _tryGetValue = tryGetValue;
            }

            public string Name { get { return _name; } }
            public ValueSource Source { get { return _source; } }

            public bool TryGetValue(out object value)
            {
                return _tryGetValue(out value);
            }

            private static TryFunc<object> Try(Func<object> getValue)
            {
                return (out object value) =>
                {
                    try { value = getValue(); return true; }
                    catch { value = null; return false; }
                };
            }
        }

        private enum ValueSource
        {
            AdditionalNode = 0,
            ProxyMember = 1,
        }
    }
}