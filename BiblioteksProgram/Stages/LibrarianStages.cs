using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblioteksProgram.Stages
{
    internal class LibrarianStages : SharedStages
    {

        BookSystem bookSystem = BookSystem.GetInstance();
        UserSystem userSystem = UserSystem.GetInstance();
        public void LibrarianProfileStage(User user)
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
        private void LibMemberStage()
        {
            Console.Clear();
            Console.WriteLine("Hantera användare");
            Console.WriteLine("1) Se användarlista\n2) Lägg till användare\n3) Ta bort användare \n4) Redigera användare \n5) Sök användare " +
                "\n6) Gör användare till bibliotikarie \n7) Gå tillbaka");
            int choice = Options(7);
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
                    FormatUserInformation(member);

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
                            }
                            else
                            {
                                Console.WriteLine("Kunde inte byta personnummer.");
                                Thread.Sleep(2000);
                            }
                            break;
                    }

                    User newUser = new User(name, password, personal_number);
                    userSystem.AddMember(newUser);
                    userSystem.RemoveMember(member);
                    Console.WriteLine("\nInformation:");
                    FormatUserInformation(newUser);
                    Thread.Sleep(2000);
                    break;
                }
                else //användaren finns inte med i systemet
                {
                    Console.WriteLine("Användaren existerar inte!");
                    if (!KeepGoing())
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
                Console.WriteLine("Denna medlem finns ej!");
                if (!KeepGoing())
                {
                    break;
                }
            } while (true);
            if (memberToRemove != null)
            {
                Console.WriteLine("Tog bort " + memberToRemove.name);
                Thread.Sleep(2000);
                userSystem.RemoveMember(memberToRemove);
            }

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
                Thread.Sleep(2000);
                userSystem.AddMember(member);
            }
        }

        private void MemberList()
        {
            Console.Clear();
            Console.WriteLine("Medlemmar i systemet:");
            foreach (User user in userSystem.GetMembers())
            {
                FormatUserInformation(user);
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
                    FormatUserInformation(user);
                }

                if (!KeepGoing())
                {
                    break;
                }

            } while (true);
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
                    FormatUserInformation(member);
                    userSystem.RemoveMember(member);
                    userSystem.AddLibrarian(member);
                    Console.WriteLine("\nKlart! Går vidare...");
                    Thread.Sleep(2000);
                    break;
                }
                else //användaren finns inte med i systemet
                {
                    Console.WriteLine("Användaren existerar inte!");
                    if (!KeepGoing())
                    {
                        break;
                    }
                }
            } while (true);
        }

        //Hantera böcker (Bibliotikarie)
        private void LibBookStage()
        {
            Console.Clear();
            Console.WriteLine("Hantera böcker");
            Console.WriteLine("1) Lägg till bok\n2) Ta bort bok\n3) Redigera bok \n4) Se boklista\n5) Sök efter bok\n6) Gå tillbaka");
            int choice = Options(6);
            switch (choice)
            {
                case 1:
                    AddBook();
                    break;
                case 2:
                    RemoveBook();
                    break;
                case 3:
                    EditBook();
                    break;
                case 4:
                    BookList(true);
                    break;
                case 5:
                    SearchBook(true);
                    break;
                case 6:
                    break;
            }
        }

        private void RemoveBook()
        {
            bool keepGoing = true;
            while (keepGoing)
            {
                //Console.Clear();
                if(BookStageOptions(true) == false)
                {
                    break;
                }

                do
                {
                    Console.WriteLine("\nVilken bok vill du ta bort?");
                    int id = ChooseID();

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

                    if (!KeepGoing())
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
                Console.Write("Sök på bok efter [titel], [författare], [ISBN], eller [genre]: ");
                string search = Console.ReadLine();
                List<Book> results = bookSystem.FindBooks(search);
                Console.WriteLine("Resultat...");
                foreach (Book book in results)
                {
                    FormatBookInformation(book, showOwner);
                }

                if (!KeepGoing())
                {
                    break;
                }

            } while (true);
        }

        private void AddBook()
        {
            Book book = null;

            do
            {
                Console.Clear();
                if (BookStageOptions(true) == false)
                {
                    break;
                }

                int id = ChooseID();

                if (bookSystem.GetTakenIds().Contains(id))
                {
                    Console.WriteLine("IDt tillhör en annan bok, vänligen välj ett annat");
                    Thread.Sleep(1000);
                    continue;
                }

                Console.Write("Titel: ");
                string title = Console.ReadLine();
                Console.Write("Författare: ");
                string author = Console.ReadLine();
                Console.Write("ISBN: ");
                string ISBN = Console.ReadLine();
                Console.Write("Genre: ");
                string genre = Console.ReadLine();

                book = new Book(id, title, author, ISBN, genre, false, false, null);

                if (WrongISBN(book))
                {
                    Console.WriteLine("Försök igen...");
                    Thread.Sleep(2000);
                    continue;
                } else
                {
                    break;
                }

            } while (true);

            if (book != null)
            {
                Console.WriteLine("Lägger till boken " + book.title + " av " + book.author);
                bookSystem.AddBook(book);
                Thread.Sleep(2000);
            }
        }

        private void EditBook()
        {
            do
            {
                Console.Clear();

                if (!BookStageOptions(true))
                {
                    break;
                }

                Console.WriteLine("Välj bok från ID: ");
                int id = ChooseID();

                Book book = bookSystem.GetBookFromID(id);
                if(book.isRented || book.isBorrowed)
                {
                    Console.WriteLine("Du kan inte redigera en bok som ägs av någon!");
                    Thread.Sleep(2000);
                    continue;
                }

                Console.WriteLine("Skriv in det du vill ändra:");
                Console.Write("Titel: ");
                string new_title = Console.ReadLine();
                Console.Write("Författare: ");
                string new_author = Console.ReadLine();
                Console.Write("ISBN: ");
                string new_ISBN = Console.ReadLine();
                Console.Write("Genre: ");
                string new_genre = Console.ReadLine();

                Book newBook = new Book(id, new_title, new_author, new_ISBN, new_genre, false, false, null);
                if (WrongISBN(newBook))
                {
                    Console.WriteLine("Försöker igen...");
                    Thread.Sleep(2000);
                    continue;
                } else
                {
                    bookSystem.RemoveBook(book);
                    bookSystem.AddBook(newBook);

                    Console.Clear();
                    Console.WriteLine("Gammal bok: ");
                    FormatBookInformation(book, true);

                    Console.WriteLine("\nRedigerad bok");
                    FormatBookInformation(newBook, true);
                    Console.ReadKey();
                    break;
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


        //Korrekt ISBN check
        private bool WrongISBN(Book book)
        {
         

            Console.WriteLine("Kollar isbn1");
            //kolla om ISBN stämmer med boken
            string correctISBN = bookSystem.WrongISBN(book);

            if (correctISBN != null)
            {
                Console.WriteLine("ISBN för denna boken är " + correctISBN + "!");
                return true; //ISBN är fel
            }
             
            if (bookSystem.ISBNIsTaken(book))
            {
                //kolla om isbn är upptaget
                Console.WriteLine("Detta ISBN tillhör en annan bok!");
                Thread.Sleep(2000);
                return true; //ISBN är taget
            }

            return false; //ISBN är rätt

        }

        //fortsätt / starta om
        public bool KeepGoing()
        {
            Console.WriteLine("\n1) Avbryt\n2) Försök igen");
            int option2 = Options(2);
            if (option2 == 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


    }
}
