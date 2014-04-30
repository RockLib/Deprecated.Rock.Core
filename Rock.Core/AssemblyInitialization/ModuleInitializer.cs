using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Registration;
using System.Linq;

namespace Rock.AssemblyInitialization
{
    public static class ModuleInitializer // Future devs: Do not change the name of this class
    {
        private static bool _hasRun;
        private static readonly object _locker = new object();

        public static void Run() // Future devs: Do not change the signature of this method
        {
            if (_hasRun)
            {
                return;
            }

            lock (_locker)
            {
                if (_hasRun)
                {
                    return;
                }

                _hasRun = true;
            }

            var conventions = new RegistrationBuilder();
            conventions
                .ForTypesDerivedFrom<IFrameworkInitializer>()
                .ExportInterfaces(t => t == typeof(IFrameworkInitializer))
                .SetCreationPolicy(CreationPolicy.NonShared);

            using (var catalog = new DirectoryCatalog(AppDomain.CurrentDomain.BaseDirectory, conventions))
            {
                using (var container = new CompositionContainer(catalog))
                {
                    var coreLibrariesFirstEntryPointLast = new CoreLibrariesFirstEntryPointLastComparer();

                    foreach (var frameworkInitializer in
                        container.GetExports<IFrameworkInitializer>()
                            .OrderBy(x => x.GetType(), coreLibrariesFirstEntryPointLast))
                    {
                        try
                        {
                            frameworkInitializer.Value.Initialize();
                        }
                        catch (Exception ex)
                        {
                            // TODO: Do something better than writing to console.
                            Console.WriteLine(ex);
                        }
                    }
                }
            }
        }

        private class CoreLibrariesFirstEntryPointLastComparer : IComparer<Type>
        {
            int IComparer<Type>.Compare(Type lhs, Type rhs)
            {
                if (lhs == rhs)
                {
                    return 0;
                }

                if (rhs == null)
                {
                    return -1;
                }

                if (lhs == null)
                {
                    return 1;
                }

                if (rhs.Assembly.GetReferencedAssemblies()
                        .Any(referenced => referenced.ToString() == lhs.Assembly.GetName().ToString()))
                {
                    return -1;
                }

                if (lhs.Assembly.GetReferencedAssemblies()
                        .Any(referenced => referenced.ToString() == rhs.Assembly.GetName().ToString()))
                {
                    return 1;
                }

                return Comparer<string>.Default.Compare(lhs.FullName, rhs.FullName);
            }
        }
    }
}