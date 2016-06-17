using NUnit.Framework;
using Rock.BackgroundErrorLogging;

namespace Rock.Core.IntegrationTests.Configuration.Xml
{
    public class LibraryLoggerConfigTests
    {
        [Test]
        public void TheLibraryLoggerDefinedInConfigIsUsed()
        {
            Assert.That(LibraryLogger.Current, Is.InstanceOf<ConsoleLibraryLogger>());
        }
    }
}