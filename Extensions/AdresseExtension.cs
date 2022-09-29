﻿using CatalogueApi.DTOs;
using laborator19_Catalog_.Models;

namespace CatalogueApi.Extensions
{
    public static class AdresseExtension
    {
        public static AddressToGet ToDto(this Address adresse)
        {
            return
                new AddressToGet
                {
                    City = adresse.City,
                    Street = adresse.Street,
                    Number = adresse.Number,
                };
        }
    }
}
