namespace Rock.AssemblyInitialization
{
    /// <summary>
    /// An interface whose implementors will be executed at the time that the Rock.Core
    /// assembly is loaded for the first time.
    /// </summary>
    public interface IFrameworkInitializer
    {
        void Initialize();
    }
}