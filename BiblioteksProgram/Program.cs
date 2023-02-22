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
            Console.WriteLine("Din Sida - " + user.name);
            Console.WriteLine("1) Böcker \n2) Konto \n3) Logga ut");
            int choice = Options(3);
            switch (choice)
            {
                case 1:
                    LibBookStage();
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
        private static void MemBookStage()
        {
            Console.Clear();
            Console.WriteLine("Bok sida");
            Console.WriteLine("1) Låna bok \n2) Reservera bok\n3) Lämna tillbaka bok \n4) Sök efter bok");
            int choice = Options(4);
            switch (choice)
            {
                case 1:
                    //låna bok
                    BorrowBook();
                    break;
                case 2:
                    //reservera bok
                    ReservBook();
                    break;
                case 3:
                    //returnera bok
                    break;
                case 4:
                    //sök efter bok
                    SearchBook();
                    break;

            }
        }

        private static void BorrowBook()
        {
            
        }

        private static void ReservBook()
        {

        }

        //Profil (Bibliotikarie)
        private static void LibrarianProfileStage(User user)
        {
            Console.Clear();
            Console.WriteLine("Din Sida - " + user.name);
            Console.WriteLine("1) Hantera böcker\n2) Hantera användare\n3) Hantera konto \n4) Logga ut");
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
                User member = userSystem.GetMember(tempMember.name, tempMember.password, tempMember.personal_number);
                if (member != null) //användaren finns i systemet
                {
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
            User memberToRemove;
            do
            {
                Console.Clear();
                User tempMember = ChooseMember();
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

                foreach (User exsistingMember in userSystem.GetMembers())
                {
                    if (exsistingMember.personal_number == member.personal_number)
                    {
                        Console.WriteLine("Detta personnummer tillhör en annan person! Försök igen");
                        memberExists = true;
                    }
                }

            } while (memberExists);
            Console.WriteLine("Lade till " + member.name + " i systemet!");
            userSystem.AddMember(member);
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
            Console.WriteLine("\nKlicka på valfri kanpp för att gå vidare ->");
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
                    BookList();
                    break;
                case 4:
                    SearchBook();
                    break;
            }
        }

        private static void RemoveBook()
        {
            do
            {
                Console.Clear();
                Console.WriteLine("Vilken bok vill du ta bort?");
                Console.Write("Titel: ");
                string title = Console.ReadLine();
                Console.Write("Författare: ");
                string author = Console.ReadLine();
                Console.Write("ISBN: ");
                string isbn = Console.ReadLine();
                Book book = new Book(title, author, isbn);
                Book bookToRemove = bookSystem.GetBook(book);
                if (bookToRemove != null)
                {
                    Console.WriteLine("Tog bort boken " + book.title + " av " + book.author);
                    bookSystem.RemoveBook(bookToRemove);
                    Thread.Sleep(2000);
                    break;
                } else
                {
                    Console.WriteLine("Boken fanns inte! \n1) Avbryt\n2) Försök igen");
                    int option = Options(2);
                    if (option == 1)
                    {
                        break;
                    }
                }
            } while (true);
        }

        private static void SearchBook()
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
                    Console.WriteLine("\nTitel: " + book.title);
                    Console.WriteLine("Författare: " + book.author);
                    Console.WriteLine("ISBN: " + book.ISBN);
                }

                Console.WriteLine("\n1) Sök igen \n2) Gå vidare");
                int choice = Options(2);
                if (choice == 2)
                {
                    break;
                }

            } while (true);
        }

        private static void BookList()
        {
            Console.Clear();
            Console.WriteLine("Tillgängliga böcker:");
            foreach (Book book in bookSystem.GetBooks())
            {
                Console.WriteLine("\nTitel: " + book.title);
                Console.WriteLine("Författare: " + book.author);
                Console.WriteLine("ISBN: " + book.ISBN);
            }

            Console.WriteLine("\nKlicka på valfri kanpp för att gå vidare ->");
            Console.ReadKey();
        }

        private static void AddBook()
        {
            Book book;
            int wrongISBN = 0; //0 är neutralt, 1 är fel, 2 är rätt

            Console.Clear();
            do
            {
                wrongISBN = 0; //återställ

                Console.Write("Titel: ");
                string title = Console.ReadLine();
                Console.Write("Författare: ");
                string author = Console.ReadLine();
                Console.Write("ISBN: ");
                string ISBN = Console.ReadLine();

                book = new Book(title, author, ISBN);
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

            bookSystem.AddBook(book);
        }


        private static void MakeLibrarian()
        {
            do
            {
                Console.Clear();
                Console.WriteLine("Välj användare att ge bibliotikarie behörigheter: ");
                User tempMember = ChooseMember();
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

        //Konto (Bibliotikare och Medlem)
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
            Console.Write("Namn: ");
            string name = Console.ReadLine();
            Console.Write("Lösenord: ");
            string password = Console.ReadLine();
            Console.Write("Personnummer: ");
            string personal_number = Console.ReadLine();
            User member = new User(name, password, personal_number);

            return member;
        }

        private static void FormatUserInformation(string name, string password, string personal_number)
        {
            Console.WriteLine("\nNamn: " + name);
            Console.WriteLine("Personnummer: " + personal_number);
            Console.WriteLine("Lösenord: " + password);
        }
    }
}
