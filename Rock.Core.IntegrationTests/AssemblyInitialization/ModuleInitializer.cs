namespace Rock.Core.IntegrationTests.AssemblyInitialization
{
    // NOTE: This module initializer exists only to ensure that the module initialization tests
    // run successfully, even if the integration tests start first. NUnit seems to create a single
    // app domain for all of its tests, so order is important. This is also the reason why this
    // project references the unit test project.
    internal static class ModuleInitializer
    {
        internal static void Run()
        {
            Rock.AssemblyInitialization.ModuleInitializer.Run();
        }
    }
}