using System;
using System.Diagnostics;
using NUnit.Framework;

namespace Rock.Core.IntegrationTests
{
    public class EntryAssemblyApplicationIdProviderTests
    {
        public class TheApplicationIdProperty
        {
            [Test]
            public void ReturnsTheNameOfTheEntryAssemblyWhenAnEntryAssemblyExists()
            {
                // These command line args are defined in the SampleApplication. See EntryAssemblyApplicationIdProviderCommand.cs and EntryAssemblyNameCommand.cs for details.
                var applicationId = RunSampleApplication("EntryAssemblyApplicationIdProvider -p=ApplicationId");
                var entryAssemblyName = RunSampleApplication("EntryAssemblyName");

                Assert.That(applicationId, Is.EqualTo(entryAssemblyName));
            }

            [Test]
            public void ReturnsTheNameOfTheLastInterestingAssemblyOnTheCallStackWhenNoEntryAssemblyExists()
            {
                var applicationIdProvider = new EntryAssemblyApplicationIdProvider();

                var result = applicationIdProvider.GetApplicationId();

                var expectedResult = GetType().Assembly.GetName().Name;

                Assert.That(result, Is.EqualTo(expectedResult));
            }
        }

        private static string RunSampleApplication(string args)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "SampleApplication.exe",
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            while (!process.StandardOutput.EndOfStream)
            {
                return process.StandardOutput.ReadLine();
            }

            throw new InvalidOperationException("Unable to read output from SampleApplication.exe");
        }
    }
}