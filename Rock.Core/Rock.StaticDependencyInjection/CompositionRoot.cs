using Rock.Conversion;
using Rock.Defaults.Implementation;
using Rock.DependencyInjection.Heuristics;
using Rock.KeyValueStores;
using Rock.Net;
using Rock.Net.Http;
using Rock.Serialization;
using Rock.StaticDependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Linq;

namespace Rock.Rock.StaticDependencyInjection
{
    internal partial class CompositionRoot : CompositionRootBase
    {
        public override void Bootstrap()
        {
            ImportFirst<IApplicationInfo>(x => Default.SetApplicationInfo(() => x));
            ImportFirst<ISerializer>(x => Default.SetBinarySerializer(() => x), "BinarySerializer");
            ImportFirst<IEndpointDetector>(x => Default.SetEndpointDetector(() => x));
            ImportFirst<IEndpointSelector>(x => Default.SetEndpointSelector(() => x));
            ImportFirst<IHttpClientFactory>(x => Default.SetHttpClientFactory(() => x));
            ImportFirst<ISerializer>(x => Default.SetJsonSerializer(() => x), "JsonSerializer");
            ImportFirst<IConvertsTo<IDictionary<string, string>>>(x => Default.SetObjectToDictionaryOfStringToStringConverter(() => x));
            ImportFirst<IConvertsTo<ExpandoObject>>(x => Default.SetObjectToExpandoObjectConverter(() => x));
            ImportFirst<IResolverConstructorSelector>(x => Default.SetResolverConstructorSelector(() => x));
            ImportFirst<IKeyValueStore>(x => Default.SetTempStorage(() => x), "TempStorage");
            ImportFirst<ISerializer>(x => Default.SetXmlSerializer(() => x), "XmlSerializer");
        }

        /// <summary>
        /// Gets a value indicating whether static dependency injection is enabled.
        /// </summary>
        public override bool IsEnabled
        {
            get
            {
                const string key = "Rock.StaticDependencyInjection.Enabled";
                var enabledValue = ConfigurationManager.AppSettings.Get(key) ?? "true";
                return enabledValue.ToLower() != "false";
            }
        }

        /// <summary>
        /// Return a collection of metadata objects that describe the export operations for a type.
        /// </summary>
        /// <param name="type">The type to get export metadata.</param>
        /// <returns>A collection of metadata objects that describe export operations.</returns>
        protected override IEnumerable<ExportInfo> GetExportInfos(Type type)
        {
            var attributes = Attribute.GetCustomAttributes(type, typeof(ExportAttribute));

            if (attributes.Length == 0)
            {
                return base.GetExportInfos(type);
            }

            return
                attributes.Cast<ExportAttribute>()
                .Select(attribute =>
                    new ExportInfo(type, attribute.Priority)
                    {
                        Disabled = attribute.Disabled,
                        Name = attribute.Name
                    });
        }
    }
}
