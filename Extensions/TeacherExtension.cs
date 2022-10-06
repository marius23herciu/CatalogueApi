using CatalogApi.DTOs;
using laborator19_Catalog_.Models;

namespace CatalogApi.Extensions
{
    public static class TeacherExtension
    {
        public static TeacherToGet ToDto(this Teacher teacher)
        {
            return
                new TeacherToGet
                {
                    Id = teacher.Id,
                    FirstName = teacher.FirstName,
                    LastName = teacher.LastName,
                    Rank = teacher.Rank,
                    SubjectId = teacher.SubjectId,
                };
        }
    }
}
