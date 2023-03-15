using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblioteksProgram
{
    internal class BookSystem
    {
        LevenshteinDistance levenshteinDistance = new LevenshteinDistance();

        string available_file = @"C:\Users\emma.dahlqvist4\Desktop\Bibliotek\BiblioteksProgram\BiblioteksProgram\Files\AvailableBooks.txt";

        private static BookSystem? instance = null;

        List<Book> available_books = new List<Book>();
        public List<Book> GetBooks() { return available_books; }

        private BookSystem()
        {
            LoadBooks();
        }

        public void AddBook(Book book)
        {
            available_books.Add(book);
            Save();
        }

        public void RemoveBook(Book book)
        {
            available_books.Remove(book);
            Save();
        }

        public List<Book> GetOwnedBooks(string personal_number)
        {
            List<Book> ownedBooks = new List<Book>();
            foreach (Book book in available_books)
            {
                if(book.owner == personal_number)
                {
                    ownedBooks.Add(book);
                }
            }
            return ownedBooks;
        }

        public List<int> GetTakenIds()
        {
            List<int> takenIds = new List<int>();
            foreach(Book book in available_books)
            {
                takenIds.Add(book.id);
            }
            return takenIds;
        }

        //få rätt upplaga
        public Book GetBook(Book book)
        {
            foreach(Book a_book in available_books)
            {
                if(a_book.title == book.title && a_book.ISBN == book.ISBN && book.author == a_book.author)
                {
                    return a_book;
                }
            }
            return null;
        }

        public Book GetBookFromID(int id)
        {
            foreach(Book book in available_books)
            {
                if(book.id == id)
                {
                    return book;
                }
            }
            return null;
        }

        public List<Book> FindBooks(string search)
        {
            int maxDistance = 2;
            List<Book> results = new List<Book>();
            foreach(Book book in available_books)
            {
                int titleDistance = levenshteinDistance.GetDistance(book.title, search);
                int authorDistance = levenshteinDistance.GetDistance(book.author, search) ;
                int ISBNDistance = levenshteinDistance.GetDistance(book.ISBN, search);
                if(titleDistance <= maxDistance || authorDistance <= maxDistance || ISBNDistance <= maxDistance)
                {
                    results.Add(book);
                }
            }

            return results;
        }

        private void Save()
        {
            string[] aBooksStrArray = available_books.Select(book => $"{book.id}|{book.title}|{book.author}|{book.ISBN}|{book.isBorrowed}|{book.isRented}|{book.owner}").ToArray();

            File.WriteAllLines(available_file, aBooksStrArray);
        }
        private void LoadBooks()
        {
            LoadFile(available_file, available_books);
        }

        private void LoadFile(string file, List<Book> list)
        {
            try
            {
                string[] fileItems = System.IO.File.ReadAllLines(file);

                foreach (string item in fileItems)
                {
                    string[] itemSplit = item.Split("|");
                    int id = int.Parse(itemSplit[0]);
                    string title = itemSplit[1];
                    string author = itemSplit[2];
                    string ISBN = itemSplit[3];
                    bool isBorrowed = bool.Parse(itemSplit[4]);
                    bool isRented = bool.Parse(itemSplit[5]);
                    string owner = itemSplit[6];

                    Book book = new Book(id, title, author, ISBN, isBorrowed, isRented, owner);
                    list.Add(book);
                }
            }
            catch (Exception ex)
            {
                //Filen var troligen tom
            }
        }

        public static BookSystem GetInstance()
        {
            if (instance == null)
            {
                instance = new BookSystem();
            }
            return instance;
        }
    }
}
