using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ConsoleApplicationBase
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = typeof(Program).Name;
            
            Run();
        }

        static void Run()
        {
            AppState.SetState(State.RUNNING);
            while (AppState.GetState() > State.IDLE)
            {  
                var consoleInput = ReadFromConsole();
                if (string.IsNullOrWhiteSpace(consoleInput)) continue;

                try
                {
                    // Create a ConsoleCommand instance:
                    var cmd = new ConsoleCommand(consoleInput);

                    // Execute the command:
                    string result = CommandHandler.Execute(cmd);

                    // Write out the result:
                    WriteToConsole(result);
                }
                catch (Exception ex)
                {
                    // OOPS! Something went wrong - Write out the problem:
                    WriteToConsole(ex.Message);
                }
            }
        }

        static void WriteToConsole(string message = "")
        {
            if(message.Length > 0)
            {
                Console.WriteLine(message);
            }
        }

        const string _readPrompt = "console> ";
        static string ReadFromConsole(string promptMessage = "")
        {
            // Show a prompt, and get input:
            Console.Write(_readPrompt + promptMessage);
            return Console.ReadLine();
        }
    }
}
