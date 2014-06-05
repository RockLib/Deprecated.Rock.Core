using System;
using ManyConsole;
using Rock;

namespace SampleApplication
{
    public class EntryAssemblyApplicationInfoCommand : ConsoleCommand
    {
        private Func<EntryAssemblyApplicationInfo, object> _getPropertyValue;

        public EntryAssemblyApplicationInfoCommand()
        {
            this.IsCommand("EntryAssemblyApplicationInfo");
            this.SkipsCommandSummaryBeforeRunning();
            this.HasRequiredOption<EntryAssemblyApplicationInfoProperty>("p|property=", "The EntryAssemblyApplicationInfo property", SetPropertyValueGetter);
        }

        private void SetPropertyValueGetter(EntryAssemblyApplicationInfoProperty entryAssemblyApplicationInfoProperty)
        {
            switch (entryAssemblyApplicationInfoProperty)
            {
                case EntryAssemblyApplicationInfoProperty.ApplicationId:
                    _getPropertyValue = info => info.ApplicationId;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("entryAssemblyApplicationInfoProperty");
            }
        }

        public override int Run(string[] remainingArguments)
        {
            var applicationInfo = new EntryAssemblyApplicationInfo();
            Console.WriteLine(_getPropertyValue(applicationInfo));
            return 0;
        }

        public enum EntryAssemblyApplicationInfoProperty
        {
            ApplicationId
        }
    }
}