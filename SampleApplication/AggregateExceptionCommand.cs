using System;
using System.Collections.Generic;
using System.IO;
using ManyConsole;
using Rock.StringFormatting;

namespace SampleApplication
{
    public class AggregateExceptionCommand : ConsoleCommand
    {
        public AggregateExceptionCommand()
        {
            this.IsCommand("AggregateException");
            this.SkipsCommandSummaryBeforeRunning();
        }

        public override int Run(string[] remainingArguments)
        {
            var exceptions = new List<Exception>();

            try
            {
                var i = 0;
                var j = 1;
                var k = j / i;
                Console.WriteLine(k);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }

            try
            {
                File.ReadAllText(@"C:\asdfasdfasdfqwertyuiop.txt");
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }

            try
            {
                object obj = null;
                obj.ToString();
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }

            try
            {
                throw new AggregateException(exceptions) { HelpLink = "http://bfy.tw/6dbx" };
            }
            catch (Exception ex)
            {
                var formattedException = ex.FormatToString();
                Console.WriteLine(formattedException);
            }

            return 0;
        }
    }
}