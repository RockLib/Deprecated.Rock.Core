using NUnit.Framework;
using Rock.Logging.Library;

namespace Rock.Core.IntegrationTests.Configuration.Xml
{
    public class LibraryLoggerConfigTests
    {
        [Test]
        public void TheLibraryLoggerDefinedInConfigIsUsed()
        {
            Assert.That(LibraryLogger.Current, Is.InstanceOf<ConsoleLibraryLogger>());
        }

        [Test]
        public void DebugEnabledIsSetFromConfig()
        {
            Assert.That(LibraryLogger.IsDebugEnabled, Is.True);
        }
    }
}