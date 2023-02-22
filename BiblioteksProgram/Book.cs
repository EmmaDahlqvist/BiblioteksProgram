using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblioteksProgram
{
    internal class Book
    {
        public string title;
        public string author;
        public string ISBN;

        public Book(string title, string author, string ISBN)
        {
            this.title = title;
            this.author = author;
            this.ISBN = ISBN;
        }
    }
}
