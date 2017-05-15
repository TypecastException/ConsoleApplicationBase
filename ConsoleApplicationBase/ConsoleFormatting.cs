using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// code was found at the C# Examples Site: http://www.csharp-examples.net/indent-string-with-spaces/

namespace ConsoleApplicationBase
{
    public class ConsoleFormatting
    {
        public static string Indent(int count)
        {
            return "".PadLeft(count);
        }
    }
}
