using System.ComponentModel.DataAnnotations;

namespace CatalogApi.DTOs
{
    public class StudentData
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Range(1, 150)]
        public int Age { get; set; }
    }
}
