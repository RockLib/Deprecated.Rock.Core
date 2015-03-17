using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Rock
{
    /// <summary>
    /// An implementation of <see cref="IApplicationInfo"/> that uses the entry
    /// assembly's name as the ApplicationId.
    /// </summary>
    public class EntryAssemblyApplicationInfo : IApplicationInfo
    {
        private static readonly IEnumerable<string> _assembliesToIgnore = new[]
        {
            typeof(EntryAssemblyApplicationInfo).Assembly.GetName().Name,
            "mscorlib",
            "Microsoft.VisualStudio.HostingProcess.Utilities",
            "nunit.core",
            "JetBrains.ReSharper.UnitTestRunner.nUnit",
            "JetBrains.ReSharper.TaskRunnerFramework",
            // TODO: add assembly names for other unit test runners
        };

        private readonly Lazy<string> _appId;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntryAssemblyApplicationInfo"/> class.
        /// </summary>
        public EntryAssemblyApplicationInfo()
        {
            var entryAssembly = new Lazy<Assembly>(GetEntryAssembly);
            _appId = new Lazy<string>(() => entryAssembly.Value.GetName().Name);
        }

        private static Assembly GetEntryAssembly()
        {
            return
                Assembly.GetEntryAssembly()
                ?? GetEntryAssemblyFromStackTrace()
                ?? ThrowNoEntryAssemblyFoundException();
        }

        private static Assembly GetEntryAssemblyFromStackTrace()
        {
            return (from frame in (new StackTrace().GetFrames() ?? Enumerable.Empty<StackFrame>())
                    select frame.GetMethod() into method
                    where method != null
                    select method.DeclaringType into declaringType
                    where declaringType != null
                    let assemblyName = declaringType.Assembly.GetName().Name
                    where !_assembliesToIgnore.Contains(assemblyName)
                    select declaringType.Assembly).LastOrDefault();
        }

        private static Assembly ThrowNoEntryAssemblyFoundException()
        {
            throw new InvalidOperationException("Unable to determine entry assembly.");
        }

        /// <summary>
        /// Gets the ID of the current application.
        /// </summary>
        public string ApplicationId
        {
            get { return _appId.Value; }
        }
    }
}