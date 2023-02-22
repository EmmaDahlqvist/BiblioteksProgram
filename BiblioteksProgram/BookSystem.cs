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
            string[] aBooksStrArray = available_books.Select(book => $"{book.title}|{book.author}|{book.ISBN}").ToArray();

            File.WriteAllLines(available_file, aBooksStrArray);
        }
        private void LoadBooks()
        {
            LoadFile(available_file, available_books);
        }

        private void LoadFile(string file, List<Book> list)
        {
            string[] fileItems = System.IO.File.ReadAllLines(file);

            foreach (string item in fileItems)
            {
                string[] itemSplit = item.Split("|");
                string title = itemSplit[0];
                string author = itemSplit[1];
                string ISBN = itemSplit[2];

                Book book = new Book(title, author, ISBN);
                list.Add(book);
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
