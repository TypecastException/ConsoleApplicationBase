using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplicationBase.Models
{
    // We'll use this for our examples:
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }


    public static class SampleData
    {
        private static List<User> _userData;
        public static List<User> Users
        {
            get
            {
                // List will be initialized with data the first time the
                // static property is accessed:
                if(_userData == null)
                {
                    _userData = CreateInitialUsers();
                }
                return _userData;
            }
        }


        // Some test data:
        static List<User> CreateInitialUsers()
        {
            var initialUsers = new List<User>()
            {
                new User { Id = 1, FirstName = "John", LastName = "Lennon" },
                new User { Id = 2, FirstName = "Paul", LastName = "McCartney" },
                new User { Id = 3, FirstName = "George", LastName = "Harrison" },
                new User { Id = 4, FirstName = "Ringo", LastName = "Starr" },
            };
            return initialUsers;

        }
    }
}
