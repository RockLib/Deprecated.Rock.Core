using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rock.AssemblyInitialization;
using Rock.Core.ModuleInitializer.UnitTestSupport.AssemblyInitialization;
using Rock.Core.UnitTests.AssemblyInitialization;

// ReSharper disable once CheckNamespace
namespace ModuleInitializerTests
{
    public class ImplementorsOfIFrameworkInitializer
    {
        [Test]
        public void HaveTheirInitializeMethodsInvokedOnce()
        {
            Assert.That(Rock_Core_UnitTests_FrameworkInitializer.InitializeInvocationCount, Is.EqualTo(1));
            Assert.That(Rock_Core_ModuleInitializer_UnitTestSupport_FrameworkInitializer.InitializeInvocationCount, Is.EqualTo(1));
        }

        /// <summary>
        /// Verify that instances of the inheritors of <see cref="IFrameworkInitializer"/> are executed
        /// in the correct order. The order should be such that an instance whose type's assembly
        /// is referenced by the assembly of another instance's type in the list should be before those
        /// other types.
        /// </summary>
        [Test]
        public void AreExecutedInAssemblyReferenceOrder()
        {
            Assert.That(FrameworkInitializerTestHelper.CalledFrameworkInitializerTypes, Is.EqualTo(TypesInAssemblyReferenceOrder));
        }

        /// <summary>
        /// Get a collection of types, in "assembly reference" order. Since the assembly of
        /// <see cref="Rock_Core_ModuleInitializer_UnitTestSupport_FrameworkInitializer"/> is referenced by
        /// the assembly of <see cref="Rock_Core_UnitTests_FrameworkInitializer"/>,
        /// <see cref="Rock_Core_ModuleInitializer_UnitTestSupport_FrameworkInitializer"/> is first, and
        /// <see cref="Rock_Core_UnitTests_FrameworkInitializer"/> is second.
        /// </summary>
        private IEnumerable<Type> TypesInAssemblyReferenceOrder
        {
            get
            {
                yield return typeof(Rock_Core_ModuleInitializer_UnitTestSupport_FrameworkInitializer);
                yield return typeof(Rock_Core_UnitTests_FrameworkInitializer);
            }
        }
    }
}