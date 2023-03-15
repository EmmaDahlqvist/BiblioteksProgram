using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblioteksProgram.Stages
{
    internal class LibrarianStages
    {

        BookSystem bookSystem = BookSystem.GetInstance();
        SharedStages sharedStages = new SharedStages();
        UserSystem userSystem = UserSystem.GetInstance();
        public void LibrarianProfileStage(User user)
        {
            Console.Clear();
            Console.WriteLine("[Bibliotikarie] Din Sida - " + user.name);
            Console.WriteLine("1) Hantera böcker\n2) Hantera användare\n3) Byt lösenord \n4) Logga ut");
            int choice = sharedStages.Options(4);
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
                    sharedStages.ChangePassword(user, "librarian");
                    LibrarianProfileStage(user);
                    break;
                case 4:
                    break;
            }
        }

        //Hantera medlemmar (Bibliotikarie)
        private void LibMemberStage()
        {
            Console.Clear();
            Console.WriteLine("Hantera användare");
            Console.WriteLine("1) Se användarlista\n2) Lägg till användare\n3) Ta bort användare \n4) Redigera användare \n5) Sök användare " +
                "\n6) Gör användare till bibliotikarie \n7) Gå tillbaka");
            int choice = sharedStages.Options(7);
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
                case 7:
                    break;
            }
        }

        private void EditMember()
        {

            do
            {
                Console.Clear();
                User tempMember = ChooseMember();

                if (tempMember == null)
                {
                    break;
                }

                User member = userSystem.GetMember(tempMember.name, tempMember.password, tempMember.personal_number);
                if (member != null) //användaren finns i systemet
                {
                    Console.Clear();
                    Console.WriteLine("Du valde användaren: ");
                    sharedStages.FormatUserInformation(member.name, member.password, member.personal_number);
                    Console.WriteLine("\n1) Ändra namn \n2) Ändra lösenord\n3) Ändra personnummer");
                    string name = member.name;
                    string password = member.password;
                    string personal_number = member.personal_number;
                    int option = sharedStages.Options(3);
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
                            }
                            else
                            {
                                Console.WriteLine("Kunde inte byta personnummer.");
                            }
                            break;
                    }

                    User newUser = new User(name, password, personal_number);
                    userSystem.AddMember(newUser);
                    userSystem.RemoveMember(member);
                    Console.WriteLine("\nInformation:");
                    sharedStages.FormatUserInformation(name, password, personal_number);
                    Thread.Sleep(2000);
                    break;
                }
                else //användaren finns inte med i systemet
                {
                    Console.WriteLine("Användaren existerar inte!\n1) Avbryt\n2) Försök igen");
                    int option2 = sharedStages.Options(2);
                    if (option2 == 1)
                    {
                        break;
                    }
                }
            } while (true);
        }

        private void RemoveMember()
        {
            User memberToRemove = null;
            do
            {
                Console.Clear();
                User tempMember = ChooseMember();
                if (tempMember == null)
                {
                    break;
                }

                memberToRemove = userSystem.GetMember(tempMember.name, tempMember.password, tempMember.personal_number);
                if (memberToRemove != null)
                {
                    break;
                }
                Console.WriteLine("Denna medlem finns ej! Vill du\n1) Avbryta\n2) Försök igen");
                int option = sharedStages.Options(2);
                if (option == 1)
                {
                    break;
                }
            } while (true);
            if (memberToRemove != null)
            {
                Console.WriteLine("Tog bort " + memberToRemove.name);
                userSystem.RemoveMember(memberToRemove);
            }
            Thread.Sleep(2000);

        }

        private void AddMember()
        {
            Console.Clear();
            bool memberExists = false;
            User member;
            do
            {
                memberExists = false; //resetta
                member = ChooseMember();
                if (member == null)
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
            if (member != null)
            {
                Console.WriteLine("Lade till " + member.name + " i systemet!");
                userSystem.AddMember(member);
            }
            Thread.Sleep(2000);
        }

        private void MemberList()
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

        private void SearchMember()
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
                int choice = sharedStages.Options(2);
                if (choice == 2)
                {
                    break;
                }

            } while (true);
        }

        //Hantera böcker (Bibliotikarie)
        private void LibBookStage()
        {
            Console.Clear();
            Console.WriteLine("Hantera böcker");
            Console.WriteLine("1) Lägg till bok\n2) Ta bort bok\n3) Se boklista\n4) Sök efter bok\n5) Gå tillbaka");
            int choice = sharedStages.Options(5);
            switch (choice)
            {
                case 1:
                    AddBook();
                    break;
                case 2:
                    RemoveBook();
                    break;
                case 3:
                    sharedStages.BookList(true);
                    break;
                case 4:
                    SearchBook(true);
                    break;
                case 5:
                    break;
            }
        }

        private void RemoveBook()
        {
            bool keepGoing = true;
            while (keepGoing)
            {
                Console.Clear();
                if(BookStageOptions() == false)
                {
                    break;
                }

                do
                {
                    int id;
                    Console.WriteLine("\nVilken bok vill du ta bort?");
                    Console.Write("ID: ");
                    try
                    {
                        id = int.Parse(Console.ReadLine());
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Vänligen välj ett korrekt siffer id!");
                        continue;
                    }

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
                    int option2 = sharedStages.Options(2);
                    if (option2 == 1)
                    {
                        keepGoing = false;
                        break;
                    }
                    break;

                } while (true);
            }
        }

        private void SearchBook(bool showOwner)
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
                    sharedStages.FormatBookInformation(book, showOwner);
                }

                Console.WriteLine("\n1) Sök igen \n2) Gå vidare");
                int choice = sharedStages.Options(2);
                if (choice == 2)
                {
                    break;
                }

            } while (true);
        }

        private void AddBook()
        {
            Book book = null;
            int wrongISBN = 0; //0 är neutralt, 1 är fel, 2 är rätt

            Console.Clear();
            do
            {
                if (BookStageOptions() == false)
                {
                    break;
                }

                wrongISBN = 0; //återställ

                Console.Write("Id: ");
                int id = 0;
                try
                {
                    id = int.Parse(Console.ReadLine());
                }
                catch (Exception ex)
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
                    }
                    else
                    {
                        wrongISBN = 2; //rätt
                    }
                }

            } while (wrongISBN != 2);

            if (book != null)
            {
                bookSystem.AddBook(book);
            }
        }

        private void MakeLibrarian()
        {
            do
            {
                Console.Clear();
                Console.WriteLine("Välj användare att ge bibliotikarie behörigheter: ");
                User tempMember = ChooseMember();

                if (tempMember == null)
                {
                    break;
                }

                User member = userSystem.GetMember(tempMember.name, tempMember.password, tempMember.personal_number);
                if (member != null) //användaren finns i systemet
                {
                    Console.WriteLine("Du valde användaren: ");
                    sharedStages.FormatUserInformation(member.name, member.password, member.personal_number);
                    userSystem.RemoveMember(member);
                    userSystem.AddLibrarian(member);
                    Console.WriteLine("\nKlart! Går vidare...");
                    Thread.Sleep(2000);
                    break;
                }
                else //användaren finns inte med i systemet
                {
                    Console.WriteLine("Användaren existerar inte!\n1) Avbryt\n2) Försök igen");
                    int option2 = sharedStages.Options(2);
                    if (option2 == 1)
                    {
                        break;
                    }
                }
            } while (true);
        }

        //Välj användare
        private User ChooseMember()
        {
            User member = null;
            while (true)
            {
                Console.WriteLine("1) Se användarlistan \n2) Sök användare\n3) Välj användare \n4) Avbryt");
                int option = sharedStages.Options(4);
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
                else if (option == 3)
                {
                    Console.Write("\nNamn: ");
                    string name = Console.ReadLine();
                    Console.Write("Personnummer: ");
                    string personal_number = Console.ReadLine();
                    Console.Write("Lösenord: ");
                    string password = Console.ReadLine();
                    member = new User(name, password, personal_number);
                    break;
                }
                else
                {
                    break;
                }

            }
            return member;
        }

        private bool BookStageOptions()
        {
            while (true)
            {
                Console.WriteLine("1) Se boklistan\n2) Sök bok \n3) Välj bok \n4) Gå tillbaka");
                int option = sharedStages.Options(4);
                if (option == 1)
                {
                    sharedStages.BookList(true);
                }
                else if (option == 2)
                {
                    sharedStages.SearchBook(true);
                }
                else if (option == 3)
                {
                    return true;
                }
                else if (option == 4)
                {
                    return false;
                }
            }
        }


    }
}
