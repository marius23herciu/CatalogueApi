
using laborator19_Catalog_.Models;
using System.ComponentModel.DataAnnotations;

namespace CatalogApi.DTOs
{
    public class TeacherToGet
    {
        [Required(ErrorMessage = "Teacher Id is required.")]
        [Range(1, int.MaxValue)]
        public int Id { get; set; }
        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Rank is required.")]
        [Range(0, 3)]
        public Rank Rank { get; set; }
        public int? SubjectId { get; set; }
    }
}
