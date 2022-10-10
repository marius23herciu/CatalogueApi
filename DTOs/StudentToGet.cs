
using laborator19_Catalog_.Models;
using System.ComponentModel.DataAnnotations;

namespace CatalogApi.DTOs
{
    public class StudentToGet
    {
        [Required(ErrorMessage = "Student Id is required.")]
        [Range(1, int.MaxValue)]
        public int Id { get; set; }
        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Age is required.")]
        [Range(1, 150)]
        public int Age { get; set; }
    }
}
