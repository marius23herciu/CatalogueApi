
using System.ComponentModel.DataAnnotations;

namespace CatalogApi.DTOs
{
    public class SubjectToCreate
    {
        [Required(ErrorMessage = "Subject's name is required.")]
        public string Name { get; set; }
    }
}
