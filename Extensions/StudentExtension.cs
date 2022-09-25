using CatalogueApi.DTOs;
using Ex1_Laborator19_.Models;

namespace CatalogueApi.Extensions
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
                    Age = student.Age
                };
        }
    }
}
