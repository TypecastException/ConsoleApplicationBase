using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApplicationBase.Models;

namespace ConsoleApplicationBase.Commands
{
    public static class Users
    {
        public static string Create(string firstName, string lastName)
        {
            Nullable<int> maxId = (from u in SampleData.Users
                                  select u.Id).Max();
            int newId = 0;
            if(maxId.HasValue)
            {
                newId = maxId.Value + 1;
            }

            var newUser = new User { Id = newId, FirstName = firstName, LastName = lastName };
            SampleData.Users.Add(newUser);
            return "";
        }


        public static string Get()
        {
            var sb = new StringBuilder();
            foreach(var user in SampleData.Users)
            {
                sb.AppendLine(ConsoleFormatting.Indent(2) + string.Format("Id:{0} {1} {2}", user.Id, user.FirstName, user.LastName));
            }
            return sb.ToString();
        }
    }
}
