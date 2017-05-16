using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplicationBase.Commands
{
    public static class TestClass
    {
        public static string RunTestMethod()
        {
            return "ran test method!";
        }

        public static string RunAnotherMethod(string name)
        {
            return "Ran another Method named: " + name;
        }
    }
}
