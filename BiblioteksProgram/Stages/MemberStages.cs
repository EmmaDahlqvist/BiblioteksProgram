using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblioteksProgram.Stages
{
    internal class MemberStages : SharedStages
    {
        BookSystem bookSystem = BookSystem.GetInstance();

        //Profil (Medlem) -> startsida
        public void MemberProfileStage(User user)
        {
            Console.Clear();
            Console.WriteLine("[Medlem] Din Sida - " + user.name);
            Console.WriteLine("1) Böcker \n2) Byt lösenord \n3) Logga ut");
            int choice = Options(3);
            switch (choice)
            {
                case 1:
                    MemBookStage(user);
                    MemberProfileStage(user);
                    break;
                case 2:
                    ChangePassword(user, "member");
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
            Console.WriteLine("1) Låna bok \n2) Reservera bok\n3) Dina böcker\n4) Sök efter bok\n5) Se boklistan \n6) Gå tillbaka");
            int choice = Options(6);
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
                    SearchBook(false);
                    break;
                case 5:
                    //boklista
                    BookList(false);
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
                        FormatBookInformation(book, true);
                        bookID.Add(book.id);
                    }
                }

                Console.WriteLine("\n1) Välj en bok \n2) Gå tillbaks");
                int option = Options(2);
                if (option == 1)
                {
                    Console.WriteLine("Välj bok ID");
                    int ID = ChooseID();

                    if (bookID.Contains(ID))
                    {
                        Book book = bookSystem.GetBookFromID(ID);
                        Console.WriteLine("\n1) Lämna tillbaks boken \n2) Gå tillbaks");
                        int option2 = Options(2);
                        if (option2 == 1)
                        {
                            Book newBook = new Book(book.id, book.title, book.author, book.ISBN, book.genre, false, false, null);
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
            string borrowOrreserv = "";
            Console.Clear();
            while (keepGoing)
            {
                if (!BookStageOptions(false))
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
                                borrowOrreserv = "Lånade";
                                book = new Book(tempBook.id, tempBook.title, tempBook.author, tempBook.ISBN, tempBook.genre, true, false, user.personal_number);
                            }
                            else //reservera
                            {
                                borrowOrreserv = "Reserverade";
                                book = new Book(tempBook.id, tempBook.title, tempBook.author, tempBook.ISBN, tempBook.genre, false, true, user.personal_number);
                            }
                            bookSystem.RemoveBook(tempBook);
                            bookSystem.AddBook(book);
                            Console.WriteLine(borrowOrreserv + " boken: " + tempBook.title + " av " + tempBook.author);
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
