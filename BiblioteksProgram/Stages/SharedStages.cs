using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblioteksProgram.Stages
{
    internal class SharedStages
    {
        UserSystem userSystem = UserSystem.GetInstance();
        BookSystem bookSystem = BookSystem.GetInstance();

        //(Bibliotikare och Medlem)
        public void ChangePassword(User user, string memberOrLibrarian)
        {
            Console.Clear();
            Console.WriteLine("Nytt lösenord:");
            string password = Console.ReadLine();
            User newUser = new User(user.name, password, user.personal_number);

            if (memberOrLibrarian == "member")
            {
                userSystem.RemoveMember(user);
                userSystem.AddMember(newUser);
            }
            else if (memberOrLibrarian == "librarian")
            {
                userSystem.RemoveLibrarian(user);
                userSystem.AddLibrarian(newUser);
            }
            Console.WriteLine("Uppdaterat!");
            Thread.Sleep(2000);
        }

        public void BookList(bool showOwner)
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


        public void SearchBook(bool showOwner)
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

        //Övriga funktioner

        public int Options(int amount)
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

        public void FormatUserInformation(User user)
        {
            Console.WriteLine("\nNamn: " + user.name);
            Console.WriteLine("Personnummer: " + user.personal_number);
            Console.WriteLine("Lösenord: " + user.password);
        }

        public void FormatBookInformation(Book book, bool showOwner)
        {
            string status = "Tillgänglig";
            Console.WriteLine("\nID: " + book.id);
            Console.WriteLine("Titel: " + book.title);
            Console.WriteLine("Författare: " + book.author);
            Console.WriteLine("ISBN: " + book.ISBN);
            Console.WriteLine("Genre: " + book.genre);
            if (book.isBorrowed == true)
            {
                status = "Utlånad";
            }
            else if (book.isRented == true)
            {
                status = "Reserverad";
            }
            Console.WriteLine("Status: " + status);
            if (showOwner)
            {
                Console.WriteLine("Ägare: " + book.owner);
            }
        }

        //Välj id (int)
        public int ChooseID()
        {
            int id;
            while (true)
            {
                Console.Write("Id: ");
                id = 0;
                try
                {
                    id = int.Parse(Console.ReadLine());
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Vänligen välj ett nummer!");
                }
            }

            return id;
        }

        //Välj bok
        public bool BookStageOptions(bool showOwner)
        {
            while (true)
            {
                Console.WriteLine("1) Se boklistan\n2) Sök bok \n3) Välj bok \n4) Gå tillbaka");
                int option = Options(4);
                if (option == 1)
                {
                    BookList(showOwner);
                }
                else if (option == 2)
                {
                    SearchBook(showOwner);
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
