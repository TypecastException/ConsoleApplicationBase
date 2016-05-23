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
        const string _commandNamespace = "ConsoleApplicationBase.Commands";
        static Dictionary<string, Dictionary<string, IEnumerable<ParameterInfo>>> _commandLibraries;

        static void Main(string[] args)
        {
            Console.Title = typeof(Program).Name;

            // Any static classes containing commands for use from the 
            // console are located in the Commands namespace. Load 
            // references to each type in that namespace via reflection:
            _commandLibraries = new Dictionary<string, Dictionary<string,
                    IEnumerable<ParameterInfo>>>();

            // Use reflection to load all of the classes in the Commands namespace:
            var q = from t in Assembly.GetExecutingAssembly().GetTypes()
                    where t.IsClass && t.Namespace == _commandNamespace
                    select t;
            var commandClasses = q.ToList();

            foreach (var commandClass in commandClasses)
            {
                // Load the method info from each class into a dictionary:
                var methods = commandClass.GetMethods(BindingFlags.Static | BindingFlags.Public);
                var methodDictionary = new Dictionary<string, IEnumerable<ParameterInfo>>();
                foreach (var method in methods)
                {
                    string commandName = method.Name;
                    methodDictionary.Add(commandName, method.GetParameters());
                }
                // Add the dictionary of methods for the current class into a dictionary of command classes:
                _commandLibraries.Add(commandClass.Name, methodDictionary);
            }
            Run();
        }


        static void Run()
        {
            while (true)
            {
                var consoleInput = ReadFromConsole();
                if (string.IsNullOrWhiteSpace(consoleInput)) continue;

                try
                {
                    // Create a ConsoleCommand instance:
                    var cmd = new ConsoleCommand(consoleInput);

                    switch (cmd.Name)
                    {
                        case "help":
                        case "?":
                        case "ayuda":
                            WriteToConsole(BuildHelpMessage());
                            break;
                        case "exit":
                        case "salir":
                            WriteToConsole("Closing program...");
                            return;
                        default:
                            // Execute the command:
                            string result = Execute(cmd);
                            // Write out the result:
                            WriteToConsole(result);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    // OOPS! Something went wrong - Write out the problem:
                    WriteToConsole(ex.Message);
                }
            }
        }


        static string Execute(ConsoleCommand command)
        {
            // Validate the class name and command name:
            // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            string badCommandMessage = string.Format(""
                + "Unrecognized command \'{0}.{1}\'. "
                + "Please type a valid command.",
                command.LibraryClassName, command.Name);

            // Validate the command name:
            if (!_commandLibraries.ContainsKey(command.LibraryClassName))
            {
                return badCommandMessage + BuildHelpMessage();
            }
            var methodDictionary = _commandLibraries[command.LibraryClassName];
            if (!methodDictionary.ContainsKey(command.Name))
            {
                return badCommandMessage + System.Environment.NewLine + BuildHelpMessage(command.LibraryClassName);
            }

            // Make sure the corret number of required arguments are provided:
            // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            var methodParameterValueList = new List<object>();
            IEnumerable<ParameterInfo> paramInfoList = methodDictionary[command.Name].ToList();

            // Validate proper # of required arguments provided. Some may be optional:
            var requiredParams = paramInfoList.Where(p => p.IsOptional == false);
            var optionalParams = paramInfoList.Where(p => p.IsOptional == true);
            int requiredCount = requiredParams.Count();
            int optionalCount = optionalParams.Count();
            int providedCount = command.Arguments.Count();

            if (requiredCount > providedCount)
            {
                var message = string.Format(
                    "Missing required parameters. {0} required, {1} optinal, {2} provided",
                    requiredCount, optionalCount, providedCount);
                string comando = command.LibraryClassName + "." + command.Name;
                message += Environment.NewLine + paramInfoList.Aggregate<ParameterInfo, String>("Call: " + comando, (total, next) => total + "  " + next.Name + " <" + next.ParameterType.Name + ">");
                return message;
            }

            // Make sure all arguments are coerced to the proper type, and that there is a 
            // value for every emthod parameter. The InvokeMember method fails if the number 
            // of arguments provided does not match the number of parameters in the 
            // method signature, even if some are optional:
            // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            if (paramInfoList.Count() > 0)
            {
                // Populate the list with default values:
                foreach (var param in paramInfoList)
                {
                    // This will either add a null object reference if the param is required 
                    // by the method, or will set a default value for optional parameters. in 
                    // any case, there will be a value or null for each method argument 
                    // in the method signature:
                    methodParameterValueList.Add(param.DefaultValue);
                }

                // Now walk through all the arguments passed from the console and assign 
                // accordingly. Any optional arguments not provided have already been set to 
                // the default specified by the method signature:
                for (int i = 0; i < command.Arguments.Count(); i++)
                {
                    var methodParam = paramInfoList.ElementAt(i);
                    var typeRequired = methodParam.ParameterType;
                    object value = null;
                    try
                    {
                        // Coming from the Console, all of our arguments are passed in as 
                        // strings. Coerce to the type to match the method paramter:
                        value = CoerceArgument(typeRequired, command.Arguments.ElementAt(i));
                        methodParameterValueList.RemoveAt(i);
                        methodParameterValueList.Insert(i, value);
                    }
                    catch (ArgumentException ex)
                    {
                        string argumentName = methodParam.Name;
                        string argumentTypeName = typeRequired.Name;
                        string message =
                            string.Format(""
                            + "The value passed for argument '{0}' cannot be parsed to type '{1}'",
                            argumentName, argumentTypeName);
                        throw new ArgumentException(message);
                    }
                }
            }

            // Set up to invoke the method using reflection:
            // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            Assembly current = typeof(Program).Assembly;

            // Need the full Namespace for this:
            Type commandLibaryClass =
                current.GetType(_commandNamespace + "." + command.LibraryClassName);

            object[] inputArgs = null;
            if (methodParameterValueList.Count > 0)
            {
                inputArgs = methodParameterValueList.ToArray();
            }
            var typeInfo = commandLibaryClass;

            // This will throw if the number of arguments provided does not match the number 
            // required by the method signature, even if some are optional:
            try
            {
                var result = typeInfo.InvokeMember(
                    command.Name,
                    BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public,
                    null, null, inputArgs);
                return result.ToString();
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }


        static object CoerceArgument(Type requiredType, string inputValue)
        {
            var requiredTypeCode = Type.GetTypeCode(requiredType);
            string exceptionMessage =
                string.Format("Cannnot coerce the input argument {0} to required type {1}",
                inputValue, requiredType.Name);

            object result = null;
            switch (requiredTypeCode)
            {
                case TypeCode.String:
                    result = inputValue;
                    break;

                case TypeCode.Int16:
                    short number16;
                    if (Int16.TryParse(inputValue, out number16))
                    {
                        result = number16;
                    }
                    else
                    {
                        throw new ArgumentException(exceptionMessage);
                    }
                    break;

                case TypeCode.Int32:
                    int number32;
                    if (Int32.TryParse(inputValue, out number32))
                    {
                        result = number32;
                    }
                    else
                    {
                        throw new ArgumentException(exceptionMessage);
                    }
                    break;

                case TypeCode.Int64:
                    long number64;
                    if (Int64.TryParse(inputValue, out number64))
                    {
                        result = number64;
                    }
                    else
                    {
                        throw new ArgumentException(exceptionMessage);
                    }
                    break;

                case TypeCode.Boolean:
                    bool trueFalse;
                    if (bool.TryParse(inputValue, out trueFalse))
                    {
                        result = trueFalse;
                    }
                    else
                    {
                        throw new ArgumentException(exceptionMessage);
                    }
                    break;

                case TypeCode.Byte:
                    byte byteValue;
                    if (byte.TryParse(inputValue, out byteValue))
                    {
                        result = byteValue;
                    }
                    else
                    {
                        throw new ArgumentException(exceptionMessage);
                    }
                    break;

                case TypeCode.Char:
                    char charValue;
                    if (char.TryParse(inputValue, out charValue))
                    {
                        result = charValue;
                    }
                    else
                    {
                        throw new ArgumentException(exceptionMessage);
                    }
                    break;

                case TypeCode.DateTime:
                    DateTime dateValue;
                    if (DateTime.TryParse(inputValue, out dateValue))
                    {
                        result = dateValue;
                    }
                    else
                    {
                        throw new ArgumentException(exceptionMessage);
                    }
                    break;
                case TypeCode.Decimal:
                    Decimal decimalValue;
                    if (Decimal.TryParse(inputValue, out decimalValue))
                    {
                        result = decimalValue;
                    }
                    else
                    {
                        throw new ArgumentException(exceptionMessage);
                    }
                    break;
                case TypeCode.Double:
                    Double doubleValue;
                    if (Double.TryParse(inputValue, out doubleValue))
                    {
                        result = doubleValue;
                    }
                    else
                    {
                        throw new ArgumentException(exceptionMessage);
                    }
                    break;
                case TypeCode.Single:
                    Single singleValue;
                    if (Single.TryParse(inputValue, out singleValue))
                    {
                        result = singleValue;
                    }
                    else
                    {
                        throw new ArgumentException(exceptionMessage);
                    }
                    break;
                case TypeCode.UInt16:
                    UInt16 uInt16Value;
                    if (UInt16.TryParse(inputValue, out uInt16Value))
                    {
                        result = uInt16Value;
                    }
                    else
                    {
                        throw new ArgumentException(exceptionMessage);
                    }
                    break;
                case TypeCode.UInt32:
                    UInt32 uInt32Value;
                    if (UInt32.TryParse(inputValue, out uInt32Value))
                    {
                        result = uInt32Value;
                    }
                    else
                    {
                        throw new ArgumentException(exceptionMessage);
                    }
                    break;
                case TypeCode.UInt64:
                    UInt64 uInt64Value;
                    if (UInt64.TryParse(inputValue, out uInt64Value))
                    {
                        result = uInt64Value;
                    }
                    else
                    {
                        throw new ArgumentException(exceptionMessage);
                    }
                    break;
                default:
                    throw new ArgumentException(exceptionMessage);
            }
            return result;
        }


        public static void WriteToConsole(string message = "")
        {
            if (message.Length > 0)
            {
                Console.WriteLine(message);
            }
        }


        const string _readPrompt = "console> ";
        public static string ReadFromConsole(string promptMessage = "")
        {
            // Show a prompt, and get input:
            Console.Write(_readPrompt + promptMessage);
            return Console.ReadLine();
        }

        static string BuildHelpMessage(string library = null)
        {
            var sb = new StringBuilder("Commands: ");
            sb.AppendLine();
            foreach (var item in _commandLibraries)
            {
                if (library != null && item.Key != library)
                    continue;
                foreach (var cmd in item.Value)
                {
                    sb.Append(ConsoleFormatting.Indent(1));
                    sb.Append(item.Key);
                    sb.Append(".");
                    sb.Append(cmd.Key);
                    sb.AppendLine();
                }

            }
            return sb.ToString();
        }
    }
}
