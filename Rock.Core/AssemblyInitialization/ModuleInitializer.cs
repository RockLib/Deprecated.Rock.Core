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
        private static readonly object _locker = new object();
        private static readonly IList<Type> _knownImplementations = new List<Type>();

        public static void Run() // Future devs: Do not change the signature of this method
        {
            lock (_locker)
            {
                var conventions = new RegistrationBuilder();
                conventions
                    .ForTypesDerivedFrom<IFrameworkInitializer>()
                    .ExportInterfaces(t => t == typeof(IFrameworkInitializer))
                    .SetCreationPolicy(CreationPolicy.NonShared);

                using (var catalog = new AggregateCatalog())
                {
                    catalog.Catalogs.Add(new DirectoryCatalog(AppDomain.CurrentDomain.BaseDirectory, conventions));
                    catalog.Catalogs.Add(new DirectoryCatalog(AppDomain.CurrentDomain.BaseDirectory, "*.exe", conventions));

                    using (var container = new CompositionContainer(catalog))
                    {
                        var coreLibrariesFirstEntryPointLast = new AssemblyReferenceOrderComparer();

                        var frameworkInitializers =
                            container
                                .GetExports<IFrameworkInitializer>()
                                .Select(export => new { Export = export, Type = export.GetType().GetGenericArguments()[0] })
                                .OrderBy(x => x.Type, coreLibrariesFirstEntryPointLast)
                                .Select(x => GetValue(x.Export)).ToList()
                                .Where(export => _knownImplementations.All(t => t != export.GetType()));

                        foreach (var frameworkInitializer in frameworkInitializers)
                        {
                            try
                            {
                                frameworkInitializer.Initialize();
                                _knownImplementations.Add(frameworkInitializer.GetType());
                            }
                            catch (Exception ex)
                            {
                                // TODO: Do something better than writing to console. (system event log?)
                                Console.WriteLine(ex);
                                throw new Exception(string.Format("The Initialize() method from {0} threw an exception. This is a fatal exception, and the application cannot continue.", frameworkInitializer.GetType()), ex);
                            }
                        }
                    }
                }
            }
        }

        private static IFrameworkInitializer GetValue(Lazy<IFrameworkInitializer> lazy)
        {
            IFrameworkInitializer value;

            try
            {
                value = lazy.Value;
            }
            catch (Exception ex)
            {
                // TODO: Do something better than writing to console. (system event log?)
                Console.WriteLine(ex);
                throw new Exception(string.Format("An exception was thrown while creating an instance of {0}. This is a fatal exception, and the application cannot continue.", typeof(IFrameworkInitializer)), ex);
            }

            return value;
        }

        /// <summary>
        /// An implementation of <see cref="IComparer{T}"/> of type <see cref="Type"/> that, when used
        /// order a list of types, puts a type whose assembly is referenced by the assembly of another 
        /// type in the list before those other types.
        /// 
        /// Given two types, A & B:
        /// - If they are the same type, they are considered equal.
        /// - If the assembly of A references the assembly of B, B is considered to be less than A.
        /// - If the assembly of B references the assembly of A, A is considered to be less than B.
        /// - Otherwise, return what the default string comparer returns for the full name of each type.
        /// </summary>
        private class AssemblyReferenceOrderComparer : IComparer<Type>
        {
            int IComparer<Type>.Compare(Type lhs, Type rhs)
            {
                if (lhs == rhs)
                {
                    return 0;
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