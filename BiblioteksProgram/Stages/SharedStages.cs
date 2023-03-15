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

        //Övrigt

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

        public void FormatUserInformation(string name, string password, string personal_number)
        {
            Console.WriteLine("\nNamn: " + name);
            Console.WriteLine("Personnummer: " + personal_number);
            Console.WriteLine("Lösenord: " + password);
        }

        public void FormatBookInformation(Book book, bool showOwner)
        {
            string status = "Tillgänglig";
            Console.WriteLine("\nID: " + book.id);
            Console.WriteLine("Titel: " + book.title);
            Console.WriteLine("Författare: " + book.author);
            Console.WriteLine("ISBN: " + book.ISBN);
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
    }
}
