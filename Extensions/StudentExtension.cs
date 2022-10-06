using CatalogApi.DTOs;
using laborator19_Catalog_.Models;

namespace CatalogApi.Extensions
{
    public static class StudentExtension
    {
        public static StudentToGet ToDto(this Student student)
        {
            return
                new StudentToGet
                {
                    Id = student.Id,
                    FirstName = student.FirstName,
                    LastName = student.LastName,
                    Age = student.Age,
                };
        }
    }
}
