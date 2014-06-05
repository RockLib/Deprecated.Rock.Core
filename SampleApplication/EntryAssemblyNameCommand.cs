using System;
using System.Reflection;
using ManyConsole;

namespace SampleApplication
{
    public class EntryAssemblyNameCommand : ConsoleCommand
    {
        public EntryAssemblyNameCommand()
        {
            this.IsCommand("EntryAssemblyName");
            this.SkipsCommandSummaryBeforeRunning();
        }

        public override int Run(string[] remainingArguments)
        {
            var entryAssemblyName = Assembly.GetEntryAssembly().GetName().Name;
            Console.WriteLine(entryAssemblyName);
            return 0;
        }
    }
}