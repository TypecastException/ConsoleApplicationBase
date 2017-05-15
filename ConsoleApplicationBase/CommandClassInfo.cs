using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplicationBase
{
    public class CommandClassInfo
    {
        public Assembly OwningAssembly { get;}
        public Dictionary<string, IEnumerable<ParameterInfo>> MethodDictionary { get; }

        public CommandClassInfo(Assembly owningAssembly, Dictionary<string, IEnumerable<ParameterInfo>> methodDict)
        {
            this.OwningAssembly = owningAssembly;
            this.MethodDictionary = methodDict;
        }

    }
}
