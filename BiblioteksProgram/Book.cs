using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblioteksProgram
{
    internal class Book
    {
        public int id;
        public string title;
        public string author;
        public string ISBN;
        public string genre;
        public bool isBorrowed { get; set; }
        public bool isRented { get; set; }
        public string owner { get; set; }

        public Book(int id,  string title, string author, string ISBN, string genre, bool isBorrowed, bool isRented, string owner)
        {
            this.id = id;
            this.title = title;
            this.author = author;
            this.ISBN = ISBN;
            this.genre = genre;
            this.isBorrowed = isBorrowed;
            this.isRented = isRented;
            this.owner = owner;
        }
    }
}
