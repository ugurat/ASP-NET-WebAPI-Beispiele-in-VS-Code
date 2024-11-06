using PersonApi.Models;

namespace PersonApi.Services
{
    public class PersonService
    {
        private static List<Person> persons = new List<Person>
        {
            new Person { Id = 1, Vorname = "Max", Nachname = "Mustermann", Alter = 50 },
            new Person { Id = 2, Vorname = "Erika", Nachname = "Musterfrau", Alter = 45 },
            new Person { Id = 3, Vorname = "Felix", Nachname = "Mustersohn", Alter = 20 }
        };

        public List<Person> GetPersons()
        {
            return persons;
        }
    }
}
