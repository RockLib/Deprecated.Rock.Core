using NUnit.Framework;
using Rock.BackgroundErrorLogging;

namespace Rock.Core.IntegrationTests.Configuration.Xml
{
    public class LibraryLoggerConfigTests
    {
        [Test]
        public void TheLibraryLoggerDefinedInConfigIsUsed()
        {
            Assert.That(BackgroundErrorLogger.Current, Is.InstanceOf<ConsoleBackgroundErrorLogger>());
        }
    }
}