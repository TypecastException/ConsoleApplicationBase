using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplicationBase
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = typeof(Program).Name;

            // We will add some set-up stuff here later...
            Run();
        }


        static void Run()
        {
            // Get input from the user:
            var consoleInput = ReadFromConsole();
            if(string.IsNullOrWhiteSpace(consoleInput))
            {
                // Nothing was provided - start over:
                Run();
            }
            try
            {
                // Create a ConsoleCommand instance:
                var cmd = new ConsoleCommand(consoleInput);

                // Execute the command:
                string result = Execute(cmd);

                // Write out the result:
                WriteToConsole(result);
            }
            catch (Exception ex)
            {
                // OOPS! Something went wrong - Write out the problem:
                WriteToConsole(ex.Message);
            }

            // Always return to Run():
            Run();
        }


        static string Execute(ConsoleCommand command)
        {
            // We'll make this more interesting shortly:
            return string.Format("Executed the {0}.{1} Command", command.LibraryClassName, command.Name);
        }


        public static void WriteToConsole(string message = "")
        {
            Console.WriteLine(ConsoleFormatting.Indent(2) + message);
        }


        const string _readPrompt = "console> ";
        public static string ReadFromConsole(string promptMessage = "")
        {
            // Show a prompt, and get input:
            Console.Write(_readPrompt + promptMessage);
            return Console.ReadLine();
        }
    }
}
