using CatalogApi.DTOs;
using laborator19_Catalog_.Models;

namespace CatalogApi.Extensions
{
    public static class SubjectExtension
    {
        public static SubjectToGet ToDto(this Subject subject)
        {
            return
                new SubjectToGet
                {
                    Id = subject.Id,
                    Name=subject.Name,
                };
        }
    }
}
