using Rock.Conversion;
using Rock.DependencyInjection;
using Rock.DependencyInjection.Heuristics;
using Rock.IO;
using Rock.KeyValueStores;
using Rock.Logging.Library;
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
            TryImportFirst<ILibraryLogger>(LibraryLogger.SetCurrent);
            
            TryImportFirst<IApplicationIdProvider>(ApplicationId.SetCurrent);
            
            TryImportFirst<IResolverConstructorSelector>(AutoContainer.SetDefaultResolverConstructorSelector);
            
            TryImportFirst<IConvertsTo<IDictionary<string, string>>>(ToDictionaryOfStringToStringExtension.SetConverter);
            TryImportFirst<IConvertsTo<ExpandoObject>>(ToExpandoObjectExtension.SetConverter);
            
            TryImportFirst<IKeyValueStore>(TempStorage.SetKeyValueStore, "TempStorage");
            
            TryImportFirst<ISerializer>(DefaultBinarySerializer.SetCurrent, "BinarySerializer");
            TryImportFirst<IEndpointDetector>(DefaultEndpointDetector.SetCurrent);
            TryImportFirst<IEndpointSelector>(DefaultEndpointSelector.SetCurrent);
            TryImportFirst<IHttpClientFactory>(DefaultHttpClientFactory.SetCurrent);
            TryImportFirst<ISerializer>(DefaultJsonSerializer.SetCurrent, "JsonSerializer");
            TryImportFirst<ISerializer>(DefaultXmlSerializer.SetCurrent, "XmlSerializer");

            LibraryLogger.UnlockCurrent();
        }

        private void TryImportFirst<TTargetType>(
            Action<TTargetType> importAction,
            string importName = null,
            ImportOptions options = null)
            where TTargetType : class
        {
            try
            {
                ImportFirst(importAction, importName, options);
            }
            catch (Exception ex)
            {
                LibraryLogger.Log(ex, "Exception caught in static dependency injection.", "Rock.Core");

                var message =
                    string.Format(
                        "Error while importing type '{0}'.{1}",
                        typeof(TTargetType),
                        importName != null
                            ? string.Format(" Import name: {0}.", importName)
                            : "");

                throw new CompositionRootException(message, ex);
            }
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
