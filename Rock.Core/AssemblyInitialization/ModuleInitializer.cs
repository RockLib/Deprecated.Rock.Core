using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;

namespace Rock.AssemblyInitialization
{
    internal static class ModuleInitializer
    {
        internal static void Run()
        {
            var container = new CompositionContainer(new DirectoryCatalog(AppDomain.CurrentDomain.BaseDirectory));
            var typeComparer = new TypeComparer();

            foreach (var initializer in container.GetExports<IAssemblyInitializer>().OrderBy(x => x.GetType(), typeComparer))
            {
                try
                {
                    initializer.Value.OnAssemblyInitialize();
                }
                catch (Exception ex)
                {
                    // TODO: Do something better than writing to console.
                    Console.WriteLine(ex);
                }
            }
        }

        private class TypeComparer : IComparer<Type>
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

                if (
                    rhs.Assembly.GetReferencedAssemblies()
                        .Any(referenced => referenced.ToString() == lhs.Assembly.GetName().ToString()))
                {
                    return -1;
                }

                if (
                    lhs.Assembly.GetReferencedAssemblies()
                        .Any(referenced => referenced.ToString() == rhs.Assembly.GetName().ToString()))
                {
                    return 1;
                }

                return Comparer<string>.Default.Compare(lhs.FullName, rhs.FullName);
            }
        }
    }
}