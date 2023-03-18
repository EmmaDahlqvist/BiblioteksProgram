using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblioteksProgram
{
    internal class UserSystem
    {
        LevenshteinDistance levenshteinDistance = new LevenshteinDistance();

        //här hanteras alla användar uppgifter/filer

        private static UserSystem? instance = null;

        string members_file = @"C:\Users\emma.dahlqvist4\Desktop\Bibliotek\BiblioteksProgram\BiblioteksProgram\Files\Members.txt";
        string librarians_file = @"C:\Users\emma.dahlqvist4\Desktop\Bibliotek\BiblioteksProgram\BiblioteksProgram\Files\Librarians.txt";

        List<User> members = new List<User>();
        public List<User> GetMembers() { return members; }

        List<User> librarians = new List<User>();
        public List<User> GetLibrarians() { return librarians; }

        private UserSystem()
        {
            LoadUsers();
        }

        public void AddMember(User member)
        {
            members.Add(member);
            Save();
        }
        public void AddLibrarian(User member)
        {
            librarians.Add(member);
            Save();
        }

        public void RemoveMember(User member)
        {
            members.Remove(member);
            Save();
        }

        public void RemoveLibrarian(User librarian)
        {
            librarians.Remove(librarian);
            Save();
        }


        private void Save()
        {
            string[] membersStringArray = members.Select(member => $"{member.name}|{member.password}|{member.personal_number}").ToArray();
            string[] librariansStringArray = librarians.Select(librarian => $"{librarian.name}|{librarian.password}|{librarian.personal_number}").ToArray();

            File.WriteAllLines(members_file, membersStringArray);
            File.WriteAllLines(librarians_file, librariansStringArray);
        }

        public void LoadUsers()
        {
            LoadFile(members_file, members);
            LoadFile(librarians_file, librarians);
        }

        private void LoadFile(string file, List<User> list)
        {
            string[] fileItems = System.IO.File.ReadAllLines(file);

            foreach (string item in fileItems)
            {
                string[] itemSplit = item.Split("|");
                string name = itemSplit[0];
                string password = itemSplit[1];
                string personal_number = itemSplit[2];

                User user = new User(name, password, personal_number);
                list.Add(user);
            }
        }

        public static UserSystem GetInstance()
        {
            if (instance == null)
            {
                instance = new UserSystem();
            }
            return instance;
        }

        public User CheckLogin(List<User> users, string password, string personal_number)
        {
            foreach (User user in users)
            {
                if (user.password == password && user.personal_number == personal_number)
                {
                    return user;

                }
            }
            return null;
        }

        public User GetMember(string name, string password, string personal_number)
        {
            foreach(User member in members)
            {
                if(member.name == name && member.password == password && member.personal_number == personal_number)
                {
                    return member;
                }
            }
            return null;
        }

        //sök funktion
        public List<User> FindUsers(string search)
        {
            List<User> results = new List<User>();
            int maxDistance = 2;
            foreach (User user in members)
            {
                int nameDistance = levenshteinDistance.GetDistance(user.name, search);
                int pNumDistance= levenshteinDistance.GetDistance(user.personal_number, search);
                int passwordDistance = levenshteinDistance.GetDistance(user.password, search);
                if (nameDistance <= maxDistance || pNumDistance <= maxDistance || passwordDistance <= maxDistance)
                {
                    results.Add(user);
                }
            }

            return results;
        }

        public bool PersonalNumberUnique(string personal_number)
        {
            foreach (User user in members)
            {
                if (user.personal_number == personal_number)
                {
                    return false; //registrering går ej igenom
                }
            }

            return true;
        }
    }
}
