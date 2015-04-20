using System;
using Rock.StringFormatting;

namespace Rock.Logging.Library
{
    public class ConsoleLibraryLogger : ILibraryLogger
    {
        public void Log(ILibraryLogMessage message)
        {
            Console.Write("LOG");
            WriteMessage(message);
        }

        public void Debug(ILibraryLogMessage message)
        {
            Console.Write("DEBUG");
            WriteMessage(message);
        }

        private static void WriteMessage(ILibraryLogMessage message)
        {
            Console.WriteLine(" {0:O} {1}", message.CreateTime, message.LibraryId);
            Console.WriteLine("    {0}", message.Message);
            Console.WriteLine("    {0}:{1}({2})", message.CallerFilePath, message.CallerMemberName, message.CallerLineNumber);

            if (message.Exception != null)
            {
                Console.WriteLine(message.Exception.FormatToString());
            }

            Console.WriteLine();
            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine();
        }
    }
}