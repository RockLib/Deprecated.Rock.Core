using System.ComponentModel.Composition;

namespace Rock.AssemblyInitialization
{
    /// <summary>
    /// An interface whose implementors will be executed at the time that the Rock.Core
    /// assembly is loaded for the first time. Note that implementors of this interface
    /// must be decorated with an <see cref="ExportAttribute"/> in order be be included
    /// for assembly initialization.
    /// </summary>
    public interface IAssemblyInitializer
    {
        void OnAssemblyInitialize();
    }
}