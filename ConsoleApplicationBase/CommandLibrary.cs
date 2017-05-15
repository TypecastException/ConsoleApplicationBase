using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplicationBase
{
    public static class CommandLibrary
    {
        const string _commandNamespace = "ConsoleApplicationBase.Commands";
        public static Dictionary<string, Dictionary<string, IEnumerable<ParameterInfo>>> Content;


        static CommandLibrary()
        {
            Content = new Dictionary<string, Dictionary<string,IEnumerable<ParameterInfo>>>();
            initialize();
        }


        static void initialize()
        {
            // Use reflection to load all of the classes in the Commands namespace:
            //var q = from t in Assembly.GetExecutingAssembly().GetTypes()
            //        where t.IsClass && t.Namespace == _commandNamespace
            //        select t;
            //var commandClasses = q.ToList();

            var commandClasses = listMatchingAssemblyTypes(Assembly.GetExecutingAssembly());

            //foreach (var commandClass in commandClasses)
            //{
            //    // Load the method info from each class into a dictionary:
            //    var methods = commandClass.GetMethods(BindingFlags.Static | BindingFlags.Public);
            //    var methodDictionary = new Dictionary<string, IEnumerable<ParameterInfo>>();
            //    foreach (var method in methods)
            //    {
            //        string commandName = method.Name;
            //        methodDictionary.Add(commandName, method.GetParameters());
            //    }
            //    // Add the dictionary of methods for the current class into a dictionary of command classes:
            //    Content.Add(commandClass.Name, methodDictionary);
            //}

            addCommands(commandClasses);
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
                    where t.IsClass && t.Namespace == _commandNamespace
                    select t;
            
            return q.ToList();
        }


        /// <summary>
        /// Adds public static methods from a list of classes
        /// to Content
        /// </summary>
        /// <param name="cmdClasses"></param>
        static void addCommands(List<Type> cmdClasses)
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
                Content.Add(commandClass.Name, methodDictionary);
            }
        }

    }
}
