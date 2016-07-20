using Rock.BackgroundErrorLogging;
using Rock.Conversion;
using Rock.DependencyInjection;
using Rock.DependencyInjection.Heuristics;
using Rock.IO;
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
            ImportFirst<IBackgroundErrorLogger>(BackgroundErrorLogger.SetCurrent);
            ImportFirst<IBackgroundErrorLogFactory>(BackgroundErrorLogger.SetBackgroundErrorLogFactory);
            
            ImportFirst<IApplicationIdProvider>(ApplicationId.SetCurrent);
            
            ImportFirst<IResolverConstructorSelector>(AutoContainer.SetDefaultResolverConstructorSelector);
            
            ImportFirst<IConvertsTo<IDictionary<string, string>>>(ToDictionaryOfStringToStringExtension.SetConverter);
            ImportFirst<IConvertsTo<ExpandoObject>>(ToExpandoObjectExtension.SetConverter);
            
            ImportFirst<IKeyValueStore>(TempStorage.SetKeyValueStore, "TempStorage");
            
            ImportFirst<ISerializer>(DefaultBinarySerializer.SetCurrent, "BinarySerializer");
            ImportFirst<IEndpointDetector>(DefaultEndpointDetector.SetCurrent);
            ImportFirst<IEndpointSelector>(DefaultEndpointSelector.SetCurrent);
            ImportFirst<IHttpClientFactory>(DefaultHttpClientFactory.SetCurrent);
            ImportFirst<ISerializer>(DefaultJsonSerializer.SetCurrent, "JsonSerializer");
            ImportFirst<ISerializer>(DefaultXmlSerializer.SetCurrent, "XmlSerializer");

            BackgroundErrorLogger.UnlockCurrent();
            BackgroundErrorLogger.UnlockBackgroundErrorLogFactory();
        }

        protected override void OnError(string message, Exception exception, ImportInfo import)
        {
            BackgroundErrorLogger.Log(exception, "Static Dependency Injection - " + message, "Rock.Core", "ImportInfo:\r\n" + import);

            base.OnError(message, exception, import);
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
