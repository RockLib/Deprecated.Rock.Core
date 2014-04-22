using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rock.AssemblyInitialization
{
    internal static class ModuleInitializer
    {
        internal static void Run()
        {
            var typeComparer = new TypeComparer();

            var assemblies =
                AppDomain.CurrentDomain.GetAssemblies().SelectMany(GetReferencedAssemblies);

            Console.WriteLine("*************");
            foreach (var aa in assemblies)
            {
                Console.WriteLine(aa.FullName);
            }
            Console.WriteLine("*************");
            
            var assemblyInitializerTypes =
                assemblies.SelectMany(a => a.GetTypes())
                    .Where(t =>
                        !t.IsAbstract
                        && typeof(IAssemblyInitializer).IsAssignableFrom(t)
                        && t.GetConstructor(Type.EmptyTypes) != null)
                    .OrderBy(x => x, typeComparer);

            foreach (var assemblyInitializerType in assemblyInitializerTypes)
            {
                try
                {
                    var assemblyInitializer = (IAssemblyInitializer)Activator.CreateInstance(assemblyInitializerType);
                    assemblyInitializer.OnAssemblyInitialize();
                }
                catch
                {
                }
            }
        }

        private static IEnumerable<Assembly> GetReferencedAssemblies(Assembly assembly)
        {
            yield return assembly;

            foreach (var referencedAssembly in
                assembly.GetReferencedAssemblies()
                    .Select(TryLoad)
                    .Where(a => a != null && !a.GlobalAssemblyCache)
                    .SelectMany(GetReferencedAssemblies))
            {
                yield return referencedAssembly;
            }
        }

        private static Assembly TryLoad(AssemblyName assemblyName)
        {
            try
            {
                return Assembly.Load(assemblyName);
            }
            catch
            {
                return null;
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

                if (rhs.Assembly.GetReferencedAssemblies().Any(referenced => referenced.ToString() == lhs.Assembly.GetName().ToString()))
                {
                    return -1;
                }

                if (lhs.Assembly.GetReferencedAssemblies().Any(referenced => referenced.ToString() == rhs.Assembly.GetName().ToString()))
                {
                    return 1;
                }

                return Comparer<string>.Default.Compare(lhs.FullName, rhs.FullName);
            }
        }
    }
}