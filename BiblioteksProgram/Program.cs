using BiblioteksProgram.Stages;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace BiblioteksProgram
{
    internal class Program
    {
        static UserSystem userSystem = UserSystem.GetInstance();
        static MemberStages memberStages = new MemberStages();
        static LibrarianStages librarianStages = new LibrarianStages();
        static SharedStages sharedStages = new SharedStages();

        static void Main(string[] args)
        {
            bool stop = false;
            while(!stop)
            {
                Console.Clear();
                Console.WriteLine("1) Logga in\n2) Registrera dig \n3) Avsluta programmet");
                int choice = sharedStages.Options(3);
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

        //Logga in
        private static void LoginStage()
        {
            Console.Clear();
            do
            {
                Console.WriteLine("Logga in:");
                Console.WriteLine("1) Medlem\n2) Bibliotikarie\n");
                int authority = sharedStages.Options(2);

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
                        //skicka vidare till medlemssidan
                        memberStages.MemberProfileStage(user);
                        break;
                    }
                    else if (authority == 2)
                    {
                        //skicka vidare till bibliotikariesidan
                        librarianStages.LibrarianProfileStage(user);
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

    }
}
