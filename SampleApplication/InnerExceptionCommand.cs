using System;
using ManyConsole;
using Rock.StringFormatting;

namespace SampleApplication
{
    public class InnerExceptionCommand : ConsoleCommand
    {
        public InnerExceptionCommand()
        {
            this.IsCommand("InnerException");
            this.SkipsCommandSummaryBeforeRunning();
        }

        public override int Run(string[] remainingArguments)
        {
            try
            {
                var i = 0;
                var j = 1;
                var k = j / i;
                Console.WriteLine(k);
            }
            catch (DivideByZeroException divideByZeroException)
            {
                try
                {
                    throw new InvalidOperationException(
@"The InnerExceptionCommand class caught a DivideByZeroException.
(This message contains multiple lines.)", divideByZeroException);
                }
                catch (InvalidOperationException invalidOperationException)
                {
                    var formattedException = invalidOperationException.FormatToString();
                    Console.WriteLine(formattedException);
                }
            }

            return 0;
        }
    }
}