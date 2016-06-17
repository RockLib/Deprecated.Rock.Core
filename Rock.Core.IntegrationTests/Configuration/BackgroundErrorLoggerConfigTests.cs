using NUnit.Framework;
using Rock.BackgroundErrorLogging;

namespace Rock.Core.IntegrationTests.Configuration
{
    public class BackgroundErrorLoggerConfigTests
    {
        [Test]
        public void TheBackgroundErrorLoggerDefinedInConfigIsUsed()
        {
            Assert.That(BackgroundErrorLogger.Current, Is.InstanceOf<ConsoleBackgroundErrorLogger>());
        }
    }
}