using CatalogueApi.DTOs;
using Ex1_Laborator19_.Models;

namespace CatalogueApi.Extensions
{
    public static class AdresseExtension
    {
        public static AdresseToGet ToDto(this Adresse adresse)
        {
            return
                new AdresseToGet
                {
                    City = adresse.City,
                    Street = adresse.Street,
                    Number = adresse.Number,
                };
        }
    }
}
