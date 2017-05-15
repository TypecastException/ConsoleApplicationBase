using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplicationBase
{
    public static class CommandLibrary
    {
        public static readonly string CommandNamespace = "ConsoleApplicationBase.Commands";
        public static Dictionary<string, CommandClassInfo> Content;


        static CommandLibrary()
        {
            Content = new Dictionary<string, CommandClassInfo>();
            initialize();
        }


        static void initialize()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var commandClasses = listMatchingAssemblyTypes(assembly);
            
            addCommands(assembly, commandClasses);

            //addCommandsFromAssemblyFile("C:\\Users\\rdoe\\Dropbox\\TEX\\Masterarbeit\\src\\consoleApp\\ConsoleApplicationBase\\TestLib\\bin\\Debug\\TestLib.dll");
        }

        /// <summary>
        /// Returns a list of types that are classes
        /// and defined inside namespace specified 
        /// in _commandNamespace
        /// </summary>
        /// <param name="assmbl">The Assembly to get the classes from</param>
        /// <returns></returns>
        static List<Type> listMatchingAssemblyTypes(Assembly assmbl)
        {
            var q = from t in assmbl.GetTypes()
                    where t.IsClass && t.Namespace == CommandNamespace
                    select t;
            
            return q.ToList();
        }

        /// <summary>
        /// Adds public static methods from a list of classes
        /// </summary>
        /// <param name="owningAssembly"></param>
        /// <param name="cmdClasses"></param>
        static void addCommands(Assembly owningAssembly,List<Type> cmdClasses)
        {
            foreach (var commandClass in cmdClasses)
            {
                var methods = commandClass.GetMethods(BindingFlags.Static | BindingFlags.Public);
                var methodDictionary = new Dictionary<string, IEnumerable<ParameterInfo>>();
                foreach (var method in methods)
                {
                    string commandName = method.Name;
                    methodDictionary.Add(commandName, method.GetParameters());
                }
                // Add the dictionary of methods for the current class into a dictionary of command classes:
                CommandLibrary.Content.Add(commandClass.Name, new CommandClassInfo(owningAssembly, methodDictionary));
            }
        }

        /// <summary>
        /// adds suitable commands from a specified assembly file
        /// </summary>
        /// <param name="assemblyFile"></param>
        public static void addCommandsFromAssemblyFile(string assemblyFile)
        {
            if (File.Exists(assemblyFile))
            {
                var extAssembly = Assembly.LoadFrom(assemblyFile);
                var extCommandCLasses = listMatchingAssemblyTypes(extAssembly);

                addCommands(extAssembly, extCommandCLasses);
            }
        }

    }
}
