using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblioteksProgram.Stages
{
    internal class MemberStages
    {
        BookSystem bookSystem = BookSystem.GetInstance();
        SharedStages sharedStages = new SharedStages();

        //Profil (Medlem)
        public void MemberProfileStage(User user)
        {
            Console.Clear();
            Console.WriteLine("[Medlem] Din Sida - " + user.name);
            Console.WriteLine("1) Böcker \n2) Byt lösenord \n3) Logga ut");
            int choice = sharedStages.Options(3);
            switch (choice)
            {
                case 1:
                    MemBookStage(user);
                    MemberProfileStage(user);
                    break;
                case 2:
                    sharedStages.ChangePassword(user, "member");
                    MemberProfileStage(user);
                    break;
                case 3:
                    break;

            }
        }

        //Boksteg (Medlem)
        private void MemBookStage(User user)
        {
            Console.Clear();
            Console.WriteLine("Bok sida");
            Console.WriteLine("1) Låna bok \n2) Reservera bok\n3) Dina böcker\n4) Sök efter bok\n5) Se boklistan \n6 Gå tillbaka");
            int choice = sharedStages.Options(6);
            switch (choice)
            {
                case 1:
                    //låna bok
                    BorrowBook(user);
                    break;
                case 2:
                    //reservera bok
                    ReservBook(user);
                    break;
                case 3:
                    YourBooks(user);
                    break;
                case 4:
                    //sök efter bok
                    sharedStages.SearchBook(false);
                    break;
                case 5:
                    //boklista
                    sharedStages.BookList(false);
                    break;
                case 6:
                    break;

            }
        }

        private void YourBooks(User user)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Dina böcker: ");
                List<Book> books = bookSystem.GetOwnedBooks(user.personal_number);
                List<int> bookID = new List<int>();
                if (books.Count == 0)
                {
                    Console.WriteLine("0...");
                }
                else
                {
                    foreach (Book book in books)
                    {
                        sharedStages.FormatBookInformation(book, true);
                        bookID.Add(book.id);
                    }
                }

                Console.WriteLine("\n1) Välj en bok \n2) Gå tillbaks");
                int option = sharedStages.Options(2);
                if (option == 1)
                {
                    Console.WriteLine("Välj bok ID");
                    int ID;
                    try
                    {
                        ID = int.Parse(Console.ReadLine());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Vänligen välj en int!");
                        Thread.Sleep(2000);
                        continue;
                    }
                    if (bookID.Contains(ID))
                    {
                        Book book = bookSystem.GetBookFromID(ID);
                        Console.WriteLine("\n1) Lämna tillbaks boken \n2) Gå tillbaks");
                        int option2 = sharedStages.Options(2);
                        if (option2 == 1)
                        {
                            Book newBook = new Book(book.id, book.title, book.author, book.ISBN, false, false, null);
                            bookSystem.AddBook(newBook);
                            bookSystem.RemoveBook(book);
                            Console.WriteLine("Lämnade tillbaks boken!");
                            Thread.Sleep(2000);
                            continue;
                        }
                        else if (option2 == 2)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Du äger inte en bok med detta ID!");
                        Thread.Sleep(2000);
                        continue;
                    }

                }
                else
                {
                    break;
                }
            }
        }

        private void BorrowBook(User user)
        {
            BorrowOrReserv(user, true);
        }

        private void ReservBook(User user)
        {
            BorrowOrReserv(user, false);
        }

        private void BorrowOrReserv(User user, bool borrow)
        {
            bool keepGoing = true;
            Console.Clear();
            while (keepGoing)
            {
                Console.WriteLine("1) Se boklistan\n2) Sök bok \n3) Välj bok\n4) Gå tillbaka");
                int option = sharedStages.Options(4);
                if (option == 1)
                {
                    sharedStages.BookList(false);
                    continue;
                }
                else if (option == 2)
                {
                    sharedStages.SearchBook(false);
                    continue;
                }
                else if (option == 4)
                {
                    break;
                }

                do
                {
                    Console.Write("\nVälj ID på boken du vill välja: ");
                    int id = int.Parse(Console.ReadLine());
                    if (!bookSystem.GetTakenIds().Contains(id))
                    {
                        Console.WriteLine("Detta ID finns inte, försök igen");
                        continue;
                    }
                    else
                    {
                        Book tempBook = bookSystem.GetBookFromID(id);
                        Book book = null;
                        if (tempBook.isBorrowed || tempBook.isRented)
                        {
                            Console.WriteLine("Denna bok ägs redan av någon annan!");
                            Thread.Sleep(2000);
                            break;
                        }
                        {
                            if (borrow) //låna
                            {
                                book = new Book(tempBook.id, tempBook.title, tempBook.author, tempBook.ISBN, true, false, user.personal_number);
                            }
                            else //reservera
                            {
                                book = new Book(tempBook.id, tempBook.title, tempBook.author, tempBook.ISBN, false, true, user.personal_number);
                            }
                            bookSystem.RemoveBook(tempBook);
                            bookSystem.AddBook(book);
                            Console.WriteLine("Lånade boken: " + tempBook.title + " av " + tempBook.author);
                            Thread.Sleep(2000);
                            keepGoing = false;
                            break;
                        }
                    }

                } while (true);

            }
        }
    }
}
