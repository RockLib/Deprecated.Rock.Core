using System;
using ManyConsole;
using Rock;

namespace SampleApplication
{
    public class EntryAssemblyApplicationIdProviderCommand : ConsoleCommand
    {
        private Func<EntryAssemblyApplicationIdProvider, object> _getPropertyValue;

        public EntryAssemblyApplicationIdProviderCommand()
        {
            this.IsCommand("EntryAssemblyApplicationIdProvider");
            this.SkipsCommandSummaryBeforeRunning();
            this.HasRequiredOption<EntryAssemblyApplicationIdProviderProperty>("p|property=", "The EntryAssemblyApplicationIdProvider property", SetPropertyValueGetter);
        }

        private void SetPropertyValueGetter(EntryAssemblyApplicationIdProviderProperty entryAssemblyApplicationIdProviderProperty)
        {
            switch (entryAssemblyApplicationIdProviderProperty)
            {
                case EntryAssemblyApplicationIdProviderProperty.ApplicationId:
                    _getPropertyValue = info => info.GetApplicationId();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("entryAssemblyApplicationIdProviderProperty");
            }
        }

        public override int Run(string[] remainingArguments)
        {
            var applicationIdProvider = new EntryAssemblyApplicationIdProvider();
            Console.WriteLine(_getPropertyValue(applicationIdProvider));
            return 0;
        }

        public enum EntryAssemblyApplicationIdProviderProperty
        {
            ApplicationId
        }
    }
}