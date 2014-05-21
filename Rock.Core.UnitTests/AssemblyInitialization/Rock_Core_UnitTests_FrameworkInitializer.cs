using Rock.AssemblyInitialization;
using Rock.Core.ModuleInitializer.UnitTestSupport.AssemblyInitialization;

namespace Rock.Core.UnitTests.AssemblyInitialization
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// This inheritor of IFrameworkInitializer counts the number of times any instance has its Initialize() method
    /// invoked. It also registers the instance call with FrameworkInitializerTestHelper when its Initializer()
    /// method is called.
    /// </summary>
    public class Rock_Core_UnitTests_FrameworkInitializer : IFrameworkInitializer
    {
        public void Initialize()
        {
            InitializeInvocationCount++;
            FrameworkInitializerTestHelper.RegisterInitializeCall(GetType());
        }

        public static int InitializeInvocationCount { get; private set; }
    }
}