using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace BiblioteksProgram
{
    internal class Program
    {
        static UserSystem userSystem = UserSystem.GetInstance();
        static BookSystem bookSystem = BookSystem.GetInstance();
        //här sköts text och program steg
        static void Main(string[] args)
        {
            bool stop = false;
            while(!stop)
            {
                Console.Clear();
                Console.WriteLine("1) Logga in\n2) Registrera dig \n3) Avsluta programmet");
                int choice = Options(3);
                switch (choice)
                {
                    case 1:
                        LoginStage();
                        break;
                    case 2:
                        RegistrationStage();
                        break;
                    case 3:
                        Console.Clear();
                        Console.WriteLine("Avslutar...");
                        stop = true;
                        break;
                }
            }

        }

        //Profil (Medlem)
        private static void MemberProfileStage(User user)
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
        private static void MemBookStage(User user)
        {
            Console.Clear();
            Console.WriteLine("Bok sida");
            Console.WriteLine("1) Låna bok \n2) Reservera bok\n3) Dina böcker\n4) Sök efter bok\n5) Se boklistan");
            int choice = Options(5);
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

            }
        }

        private static void YourBooks(User user)
        {
            while(true)
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
                    int ID;
                    try
                    {
                        ID = int.Parse(Console.ReadLine());
                    } catch(Exception ex)
                    {
                        Console.WriteLine("Vänligen välj en int!");
                        Thread.Sleep(2000);
                        continue;
                    }
                    if (bookID.Contains(ID))
                    {
                        Book book = bookSystem.GetBookFromID(ID);
                        Console.WriteLine("\n1) Lämna tillbaks boken \n2) Gå tillbaks");
                        int option2 = Options(2);
                        if(option2 == 1)
                        {
                            Book newBook = new Book(book.id, book.title, book.author, book.ISBN, false, false, null);
                            bookSystem.AddBook(newBook);
                            bookSystem.RemoveBook(book);
                            Console.WriteLine("Lämnade tillbaks boken!");
                            Thread.Sleep(2000);
                            continue;
                        } else if(option2 == 2)
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

                } else
                {
                    break;
                }
            }
        }

        private static void BorrowBook(User user)
        {
            BorrowOrReserv(user, true);
        }

        private static void ReservBook(User user)
        {
            BorrowOrReserv(user, false);
        }

        private static void BorrowOrReserv(User user, bool borrow)
        {
            bool keepGoing = true;
            Console.Clear();
            while (keepGoing)
            {
                Console.WriteLine("1) Se boklistan\n2) Sök bok \n3) Välj bok\n4) Gå tillbaka");
                int option = Options(4);
                if (option == 1)
                {
                    BookList(false);
                    continue;
                }
                else if (option == 2)
                {
                    SearchBook(false);
                    continue;
                } else if(option == 4)
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
                        if(tempBook.isBorrowed || tempBook.isRented)
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
                            keepGoing = false;
                            break;
                        }
                    }

                } while (true);

            }
        }

        //Profil (Bibliotikarie)
        private static void LibrarianProfileStage(User user)
        {
            Console.Clear();
            Console.WriteLine("[Bibliotikarie] Din Sida - " + user.name);
            Console.WriteLine("1) Hantera böcker\n2) Hantera användare\n3) Byt lösenord \n4) Logga ut");
            int choice = Options(4);
            switch (choice)
            {
                case 1:
                    LibBookStage();
                    LibrarianProfileStage(user);
                    break;
                case 2:
                    LibMemberStage();
                    LibrarianProfileStage(user);
                    break;
                case 3:
                    //konto stage
                    ChangePassword(user, "librarian");
                    LibrarianProfileStage(user);
                    break;
                case 4:
                    break;
            }
        }

        //Hantera medlemmar (Bibliotikarie)
        private static void LibMemberStage()
        {
            Console.Clear();
            Console.WriteLine("Hantera användare");
            Console.WriteLine("1) Se användarlista\n2) Lägg till användare\n3) Ta bort användare \n4) Redigera användare \n5) Sök användare " +
                "\n6) Gör användare till bibliotikarie");
            int choice = Options(6);
            switch (choice)
            {
                case 1:
                    //se lista över alla mem
                    MemberList();
                    break;
                case 2:
                    AddMember();
                    break;
                case 3:
                    //ta bort mem
                    RemoveMember();
                    break;
                case 4:
                    //redigera medlem
                    EditMember();
                    break;
                case 5:
                    //sök medlem
                    SearchMember();
                    break;
                case 6:
                    //gör till bibliotikarie
                    MakeLibrarian();
                    break;
            }
        }

        private static void EditMember()
        {

            do
            {
                Console.Clear();
                User tempMember = ChooseMember();

                if(tempMember == null)
                {
                    break;
                }

                User member = userSystem.GetMember(tempMember.name, tempMember.password, tempMember.personal_number);
                if (member != null) //användaren finns i systemet
                {
                    Console.Clear();
                    Console.WriteLine("Du valde användaren: ");
                    FormatUserInformation(member.name, member.password, member.personal_number);
                    Console.WriteLine("\n1) Ändra namn \n2) Ändra lösenord\n3) Ändra personnummer");
                    string name = member.name;
                    string password = member.password;
                    string personal_number = member.personal_number;
                    int option = Options(3);
                    switch (option)
                    {
                        case 1:
                            Console.Write("Nytt namn: ");
                            name = Console.ReadLine();
                            break;
                        case 2:
                            Console.Write("Nytt lösenord: ");
                            password = Console.ReadLine();
                            break;
                        case 3:
                            Console.Write("Nytt personnummer: ");
                            string temp_personal_number = Console.ReadLine();
                            if (userSystem.PersonalNumberUnique(temp_personal_number))
                            {
                                personal_number = temp_personal_number;
                            } else
                            {
                                Console.WriteLine("Kunde inte byta personnummer.");
                            }
                            break;
                    }

                    User newUser = new User(name, password, personal_number);
                    userSystem.AddMember(newUser);
                    userSystem.RemoveMember(member);
                    Console.WriteLine("\nInformation:");
                    FormatUserInformation(name, password, personal_number);
                    Thread.Sleep(2000);
                    break;
                }
                else //användaren finns inte med i systemet
                {
                    Console.WriteLine("Användaren existerar inte!\n1) Avbryt\n2) Försök igen");
                    int option2 = Options(2);
                    if (option2 == 1)
                    {
                        break;
                    }
                }
            } while (true);
        }

        private static void RemoveMember()
        {
            User memberToRemove = null;
            do
            {
                Console.Clear();
                User tempMember = ChooseMember();
                if(tempMember == null)
                {
                    break;
                }

                memberToRemove = userSystem.GetMember(tempMember.name, tempMember.password, tempMember.personal_number);
                if(memberToRemove != null)
                {
                    break;
                }
                Console.WriteLine("Denna medlem finns ej! Vill du\n1) Avbryta\n2) Försök igen");
                int option = Options(2);
                if(option == 1)
                {
                    break;
                }
            } while (true);
            if(memberToRemove != null)
            {
                Console.WriteLine("Tog bort " + memberToRemove.name);
                userSystem.RemoveMember(memberToRemove);
            }
            Thread.Sleep(2000);

        }

        private static void AddMember()
        {
            Console.Clear();
            bool memberExists = false;
            User member;
            do
            {
                memberExists = false; //resetta
                member = ChooseMember();
                if(member == null)
                {
                    break;
                }

                foreach (User exsistingMember in userSystem.GetMembers())
                {
                    if (exsistingMember.personal_number == member.personal_number)
                    {
                        Console.WriteLine("Detta personnummer tillhör en annan person! Försök igen");
                        memberExists = true;
                    }
                }

            } while (memberExists);
            if(member != null)
            {
                Console.WriteLine("Lade till " + member.name + " i systemet!");
                userSystem.AddMember(member);
            }
            Thread.Sleep(2000);
        }

        private static void MemberList()
        {
            Console.Clear();
            Console.WriteLine("Medlemmar i systemet:");
            foreach (User user in userSystem.GetMembers())
            {
                Console.WriteLine("\nNamn: " + user.name);
                Console.WriteLine("Personnummer: " + user.personal_number);
                Console.WriteLine("Lösenord: " + user.password);
            }
            Console.WriteLine("\nKlicka på valfri knapp för att gå vidare ->");
            Console.ReadKey();
        }

        private static void SearchMember()
        {
            do
            {
                Console.Clear();
                Console.Write("Sök på medlem efter [namn], [personnummer], eller [lösenord]: ");
                string search = Console.ReadLine();
                List<User> results = userSystem.FindUsers(search);
                Console.WriteLine("Resultat...");
                foreach (User user in results)
                {
                    Console.WriteLine("\nNamn: " + user.name);
                    Console.WriteLine("Personnummer: " + user.personal_number);
                    Console.WriteLine("Lösenord: " + user.password);
                }

                Console.WriteLine("\n1) Sök igen \n2) Gå vidare");
                int choice = Options(2);
                if (choice == 2)
                {
                    break;
                }

            } while (true);
        }

        //Hantera böcker (Bibliotikarie)
        private static void LibBookStage()
        {
            Console.Clear();
            Console.WriteLine("Hantera böcker");
            Console.WriteLine("1) Lägg till bok\n2) Ta bort bok\n3) Se boklista\n4) Sök efter bok");
            int choice = Options(4);
            switch (choice)
            {
                case 1:
                    AddBook();
                    break;
                case 2:
                    RemoveBook();
                    break;
                case 3:
                    BookList(true);
                    break;
                case 4:
                    SearchBook(true);
                    break;
            }
        }

        private static void RemoveBook()
        {
            bool keepGoing = true;
            while (keepGoing)
            {
                Console.Clear();
                Console.WriteLine("1) Se boklistan\n2) Ta bort bok");
                int option = Options(2);
                if (option == 1)
                {
                    BookList(true);
                }

                do
                {
                    Console.WriteLine("\nVilken bok vill du ta bort?");
                    Console.Write("ID: ");
                    int id = int.Parse(Console.ReadLine());

                    Book bookToRemove = bookSystem.GetBookFromID(id);
                    if (bookToRemove != null)
                    {
                        if (bookToRemove.isRented || bookToRemove.isBorrowed)
                        {
                            Console.WriteLine("Boken är utlånad och kan ej tas bort");
                        }
                        else
                        {
                            Console.WriteLine("Tog bort boken " + bookToRemove.title + " av " + bookToRemove.author);
                            bookSystem.RemoveBook(bookToRemove);
                            Thread.Sleep(2000);
                            keepGoing = false;
                            break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Boken fanns inte!");
                    }
                    Console.WriteLine("\n1) Avbryt\n2) Försök igen");
                    int option2 = Options(2);
                    if (option2 == 1)
                    {
                        keepGoing = false;
                        break;
                    }
                    break;

                } while (true);
            }
        }

        private static void SearchBook(bool showOwner)
        {
            do
            {
                Console.Clear();
                Console.Write("Sök på bok efter [titel], [författare], eller [ISBN]: ");
                string search = Console.ReadLine();
                List<Book> results = bookSystem.FindBooks(search);
                Console.WriteLine("Resultat...");
                foreach (Book book in results)
                {
                    FormatBookInformation(book, showOwner);
                }

                Console.WriteLine("\n1) Sök igen \n2) Gå vidare");
                int choice = Options(2);
                if (choice == 2)
                {
                    break;
                }

            } while (true);
        }

        private static void AddBook()
        {
            Book book = null;
            int wrongISBN = 0; //0 är neutralt, 1 är fel, 2 är rätt

            Console.Clear();
            do
            {
                wrongISBN = 0; //återställ

                Console.Write("Id: ");
                int id = 0;
                try
                {
                    id = int.Parse(Console.ReadLine());
                } catch(Exception ex)
                {
                    Console.WriteLine("Vänligen välj ett nummer!");
                    Thread.Sleep(2000);
                    continue;
                }
                if (bookSystem.GetTakenIds().Contains(id))
                {
                    Console.WriteLine("IDt tillhör en annan bok, vänligen välj ett annat");
                    continue;
                }
                Console.Write("Titel: ");
                string title = Console.ReadLine();
                Console.Write("Författare: ");
                string author = Console.ReadLine();
                Console.Write("ISBN: ");
                string ISBN = Console.ReadLine();

                book = new Book(id, title, author, ISBN, false, false, null);
                List<Book> a_books = bookSystem.GetBooks();

                List<string> takenISBN = new List<string>();

                foreach (Book a_book in a_books)
                {
                    takenISBN.Add(a_book.ISBN);
                    if (a_book.title == title && a_book.author == author)
                    {
                        if (a_book.ISBN != ISBN)
                        {
                            Console.WriteLine($"ISBN för denna bok är {a_book.ISBN}, vänligen försök igen!\n");
                            wrongISBN = 1; //fel
                            break;
                        }
                        else
                        {
                            wrongISBN = 2; //rätt
                        }
                    }
                }
                if (wrongISBN == 0)
                {
                    //ISBN finns men tillgör en annan titel
                    if (takenISBN.Contains(ISBN))
                    {
                        Console.WriteLine("Detta ISBN tillhör en annan bok!");
                        wrongISBN = 1; //fel
                    } else
                    {
                        wrongISBN = 2; //rätt
                    }
                }

            } while (wrongISBN != 2);

            if(book != null)
            {
                bookSystem.AddBook(book);
            }
        }

        private static void MakeLibrarian()
        {
            do
            {
                Console.Clear();
                Console.WriteLine("Välj användare att ge bibliotikarie behörigheter: ");
                User tempMember = ChooseMember();

                if(tempMember == null)
                {
                    break;
                }

                User member = userSystem.GetMember(tempMember.name, tempMember.password, tempMember.personal_number);
                if (member != null) //användaren finns i systemet
                {
                    Console.WriteLine("Du valde användaren: ");
                    FormatUserInformation(member.name, member.password, member.personal_number);
                    userSystem.RemoveMember(member);
                    userSystem.AddLibrarian(member);
                    Console.WriteLine("\nKlart! Går vidare...");
                    Thread.Sleep(2000);
                    break;
                }
                else //användaren finns inte med i systemet
                {
                    Console.WriteLine("Användaren existerar inte!\n1) Avbryt\n2) Försök igen");
                    int option2 = Options(2);
                    if (option2 == 1)
                    {
                        break;
                    }
                }
            } while (true);
        }

        //(Bibliotikare och Medlem)
        private static void ChangePassword(User user, string memberOrLibrarian)
        {
            Console.Clear();
            Console.WriteLine("Nytt lösenord:");
            string password = Console.ReadLine();
            User newUser = new User(user.name, password, user.personal_number);
            
            if (memberOrLibrarian == "member")
            {
                userSystem.RemoveMember(user);
                userSystem.AddMember(newUser);
            } else if(memberOrLibrarian == "librarian")
            {
                userSystem.RemoveLibrarian(user);
                userSystem.AddLibrarian(newUser);
            }
            Console.WriteLine("Uppdaterat!");
            Thread.Sleep(2000);
        }

        private static void BookList(bool showOwner)
        {
            Console.Clear();
            Console.WriteLine("Tillgängliga böcker:");
            foreach (Book book in bookSystem.GetBooks())
            {
                FormatBookInformation(book, showOwner);
            }

            Console.WriteLine("\nKlicka på valfri knapp för att gå vidare ->");
            Console.ReadKey();
        }

        //Logga in
        private static void LoginStage()
        {
            Console.Clear();
            do
            {
                Console.WriteLine("Logga in:");
                Console.WriteLine("1) Medlem\n2) Bibliotikarie\n");
                int authority = Options(2);

                Console.Write("Lösenord: ");
                string password = Console.ReadLine();
                Console.Write("Personnummer: ");
                string personal_number = Console.ReadLine();

                User user = null;
                if(authority == 1)
                {
                    user = userSystem.CheckLogin(userSystem.GetMembers(), password, personal_number);
                } else if(authority == 2)
                {
                    user = userSystem.CheckLogin(userSystem.GetLibrarians(), password, personal_number);
                }

                //kolla om användaren finns
                if (user != null)
                {
                    if (authority == 1)
                    {
                        MemberProfileStage(user);
                        break;
                    }
                    else if (authority == 2)
                    {
                        LibrarianProfileStage(user);
                        break;
                    }
                }

                Console.WriteLine("Uppgifterna stämmer ej, försök igen");
            } while (true);
        }

        //Registrering
        public static void RegistrationStage()
        {
            Console.Clear();
            Console.WriteLine("Registrering:");

            do
            {
                Console.Write("Namn: ");
                string name = Console.ReadLine();
                Console.Write("Lösenord: ");
                string password = Console.ReadLine();
                Console.Write("Personnummer: ");
                string personal_number = Console.ReadLine();

                User user = new User(name, password, personal_number);
                if (userSystem.PersonalNumberUnique(personal_number))
                {
                    userSystem.AddMember(user);
                    Console.WriteLine("Du är nu registrerad " + name);
                    Thread.Sleep(2000);
                    LoginStage();
                    break;
                }

                Console.WriteLine("Personnummret måste vara unikt! Försök igen");

            } while (true);
        }

        private static int Options(int amount)
        {
            int input = 0;
            bool correct = false;
            do
            {
                try
                {
                    input = int.Parse(Console.ReadLine());
                    for (int i = 1; i <= amount; i++)
                    {
                        if (input == i)
                        {
                            correct = true;
                        }
                    }
                    if (!correct)
                    {
                        Console.WriteLine("Välj ett efterfrågat alternativ!");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Vänligen välj en int!");
                }
            } while (!correct);

            return input;
        }

        private static User ChooseMember()
        {
            User member = null;
            while (true)
            {
                Console.WriteLine("1) Se användarlistan \n2) Sök användare\n3) Välj användare \n4) Avbryt");
                int option = Options(4);
                if (option == 1)
                {
                    MemberList();
                    continue;
                }
                else if (option == 2)
                {
                    SearchMember();
                    continue;
                }
                else if(option == 3)
                {
                    Console.Write("\nNamn: ");
                    string name = Console.ReadLine();
                    Console.Write("Personnummer: ");
                    string personal_number = Console.ReadLine();
                    Console.Write("Lösenord: ");
                    string password = Console.ReadLine();
                    member = new User(name, password, personal_number);
                    break;
                } else
                {
                    break;
                }

            }
            return member;
        }

        private static void FormatUserInformation(string name, string password, string personal_number)
        {
            Console.WriteLine("\nNamn: " + name);
            Console.WriteLine("Personnummer: " + personal_number);
            Console.WriteLine("Lösenord: " + password);
        }

        private static void FormatBookInformation(Book book, bool showOwner)
        {
            string status = "Tillgänglig";
            Console.WriteLine("\nID: " + book.id);
            Console.WriteLine("Titel: " + book.title);
            Console.WriteLine("Författare: " + book.author);
            Console.WriteLine("ISBN: " + book.ISBN);
            if(book.isBorrowed == true)
            {
                status = "Utlånad";
            } else if(book.isRented == true)
            {
                status = "Reserverad";
            }
            Console.WriteLine("Status: " + status);
            if (showOwner)
            {
                Console.WriteLine("Ägare: " + book.owner);
            }
        }
    }
}
