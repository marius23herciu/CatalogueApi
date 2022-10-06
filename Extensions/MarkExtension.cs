using CatalogApi.DTOs;
using laborator19_Catalog_.Models;

namespace CatalogApi.Extensions
{
    public static class MarkExtension
    {
        public static MarkToGet ToDto(this Mark mark)
        {
            return
                new MarkToGet
                {
                    Id = mark.Id,
                    DateTime = mark.DateTime,
                    Value = mark.Value,
                };
        }
    }
}
