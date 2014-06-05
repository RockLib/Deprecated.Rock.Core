using System;
using ManyConsole;

namespace SampleApplication
{
    class Program
    {
        static int Main(string[] args)
        {
            return ConsoleCommandDispatcher.DispatchCommand(
                ConsoleCommandDispatcher.FindCommandsInSameAssemblyAs(typeof(Program)),
                args,
                Console.Out);
        }
    }
}
