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

        //Bokens titel och författare stämmer överens med en annan bok
        public bool BooksWithSameTitle(Book book)
        {
            foreach (Book a_books in available_books)
            {
                if (a_books.title == book.title && a_books.author == book.author)
                {
                    return true;
                }
            }
            return false;
        }

        public string WrongISBN(Book book)
        {
            string correctISBN = null;
            foreach(Book a_books in available_books)
            {
                if(a_books.title == book.title && a_books.author == book.author && book.id != a_books.id)
                {
                    if(a_books.ISBN == book.ISBN)
                    {
                        //STÄMMER
                        return correctISBN; // returnera som null
                    } else
                    {
                        //STÄMMER INTE => returnera det korrekta
                        return a_books.ISBN;
                    }
                }
            }

            //Boken fanns inte ens med
            return correctISBN;
        }

        //bör kombineras med booktitleexists och wrongisbn
        public bool ISBNIsTaken(Book book)
        {
            foreach(Book a_books in available_books)
            {
                if(a_books.ISBN == book.ISBN && a_books.id != book.id)
                {
                    return true;
                }
            }

            return false;
        }

        //Sök funktion
        public List<Book> FindBooks(string search)
        {
            int maxDistance = 2;
            List<Book> results = new List<Book>();
            foreach(Book book in available_books)
            {
                int titleDistance = levenshteinDistance.GetDistance(book.title, search);
                int authorDistance = levenshteinDistance.GetDistance(book.author, search) ;
                int ISBNDistance = levenshteinDistance.GetDistance(book.ISBN, search);
                int genreDistance = levenshteinDistance.GetDistance(book.genre, search) ;
                if(titleDistance <= maxDistance || authorDistance <= maxDistance || ISBNDistance <= maxDistance || genreDistance <= maxDistance)
                {
                    results.Add(book);
                }
            }

            return results;
        }

        private void Save()
        {
            string[] aBooksStrArray = available_books.Select(book => $"{book.id}|{book.title}|{book.author}|{book.ISBN}|{book.genre}|{book.isBorrowed}|{book.isRented}|{book.owner}").ToArray();

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
                    string genre = itemSplit[4];
                    bool isBorrowed = bool.Parse(itemSplit[5]);
                    bool isRented = bool.Parse(itemSplit[6]);
                    string owner = itemSplit[7];

                    Book book = new Book(id, title, author, ISBN, genre, isBorrowed, isRented, owner);
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
