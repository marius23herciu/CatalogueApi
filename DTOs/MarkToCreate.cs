
using System.ComponentModel.DataAnnotations;

namespace CatalogApi.DTOs
{
    public class MarkToCreate
    {
        [Required(ErrorMessage = "Value is required.")]
        [Range(1,10)]
        public int Value { get; set; }
        //public DateTime DateTime { get; set; } = DateTime.Now;
        [Required(ErrorMessage = "Subcet's Id is required.")]
        [Range(1, int.MaxValue)]
        public int SubjectId { get; set; }
        [Required(ErrorMessage = "Student's Id is required.")]
        [Range(1, int.MaxValue)]
        public int StudentId { get; set; }
    }
}
