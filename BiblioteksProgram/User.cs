using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblioteksProgram
{
    internal class User
    {
        public string name;
        public string password;
        public string personal_number;

        public User(string name, string password, string personal_number)
        {
            this.name = name;
            this.password = password;
            this.personal_number = personal_number;
        }
    }
}
