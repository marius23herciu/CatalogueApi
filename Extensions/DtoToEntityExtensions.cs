using CatalogApi.DTOs;
using laborator19_Catalog_.Models;

namespace CatalogApi.Extensions
{
    public static class DtoToEntityExtensions
    {
        public static Student ToEntity(this StudentToCreate studentToCreate) =>
           new Student
           {
            FirstName = studentToCreate.FirstName,
            LastName = studentToCreate.LastName,
            Age = studentToCreate.Age
           };
        public static Subject ToEntity(this SubjectToCreate subjectToCreate) =>
           new Subject
           {
               Name = subjectToCreate.Name,
           };
        public static Mark ToEntity(this MarkToCreate markToCreate) =>
           new Mark
           {
               Value = markToCreate.Value,
           };
        public static Address ToEntity(this AddressToCreate addressToCreate) =>
            new Address
            {
                City = addressToCreate.City,
                Street = addressToCreate.Street,
                Number = addressToCreate.Number
            };
    }
}
