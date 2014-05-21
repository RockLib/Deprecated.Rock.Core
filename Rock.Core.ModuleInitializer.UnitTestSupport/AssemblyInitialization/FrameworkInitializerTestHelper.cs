using System;
using System.Collections.Generic;
using Rock.AssemblyInitialization;

namespace Rock.Core.ModuleInitializer.UnitTestSupport.AssemblyInitialization
{
    public static class FrameworkInitializerTestHelper
    {
        private static readonly List<Type> _calledFrameworkInitializers = new List<Type>();

        /// <summary>
        /// A method to be called when an inheritor of <see cref="IFrameworkInitializer"/> has its
        /// <see cref="IFrameworkInitializer.Initialize"/> method called.
        /// </summary>
        public static void RegisterInitializeCall(Type frameworkInitializerType)
        {
            _calledFrameworkInitializers.Add(frameworkInitializerType);
        }

        /// <summary>
        /// Gets a collection of types that are in the order that they were added via the
        /// <see cref="RegisterInitializeCall"/> method.
        /// </summary>
        public static IEnumerable<Type> CalledFrameworkInitializerTypes
        {
            get { return _calledFrameworkInitializers; }
        }
    }
}