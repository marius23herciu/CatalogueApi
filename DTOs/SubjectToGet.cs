using laborator19_Catalog_.Models;
using System.ComponentModel.DataAnnotations;

namespace CatalogApi.DTOs
{
    public class SubjectToGet
    {
        [Required(ErrorMessage = "Subject Id is required.")]
        [Range(1, int.MaxValue)]
        public int Id { get; set; }
       
        [Required(ErrorMessage = "Subject's name is required.")]
        public string Name { get; set; }
        
    }
}
