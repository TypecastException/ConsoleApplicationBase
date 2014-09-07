using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplicationBase.Commands
{
    public static class DefaultCommands
    {
        public static string DoSomething(int id, string data)
        {
            return string.Format("I did something to the record Id {0} and save the data {1}", id, data);
        }


        public static string DoSomethingElse(DateTime date)
        {
            return string.Format("I did something else on {0}", date);
        }
    }
}
