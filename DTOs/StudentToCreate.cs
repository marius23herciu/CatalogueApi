using Ex1_Laborator19_.Models;

namespace CatalogueApi.DTOs
{
    public class StudentToCreate
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public int AdresseId { get; set; }
    }
}
