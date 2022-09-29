using CatalogueApi.DTOs;
using CatalogueApi.Extensions;
using laborator19_Catalog_.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace CatalogueApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {

        /*• Obtinerea tuturor studentilor*/
        /// <summary>
        /// Returns all students with name and age.
        /// </summary>
        /// <returns></returns>
        [HttpGet("all")]
        public IEnumerable<StudentToGet> GetAllStudents()
        {
            using var ctx = new CatalogueDbContext();
            return ctx.Students.Select(s => s.ToDto()).ToList();
        }
        /*• Obtinerea unui student dupa ID*/
        /// <summary>
        /// Returns name and age of a selected student by his Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public StudentToGet GetStudentById([FromRoute] int id)
        {
            using var ctx = new CatalogueDbContext();
            var student = ctx.Students.Where(s => s.Id == id).FirstOrDefault();
            return student.ToDto();
        }
        /*• Creeare student*/
        /// <summary>
        /// Creates and adds a student in database.
        /// </summary>
        /// <param name="studentToCreate"></param>
        /// <returns></returns>
        [HttpPost("create")]
        public StudentToGet CreateStudent([FromBody] StudentToCreate studentToCreate)
        {
            using var ctx = new CatalogueDbContext();

            var newStudent = new Student
            {
                FirstName = studentToCreate.FirstName,
                LastName = studentToCreate.LastName,
                Age = studentToCreate.Age,
            };

            ctx.Add(newStudent);
            ctx.SaveChanges();
            return newStudent.ToDto();
        }
        /*• Stergere student*/
        /// <summary>
        /// Deletes a student from Db.
        /// </summary>
        /// <param name="studentId"></param>
        [HttpDelete("delete{studentId}")]
        public void DeleteStudent([FromBody] int studentId)
        {
            using var ctx = new CatalogueDbContext();
            var studentToRemove = ctx.Students.Include(a => a.Adresse).Where(s => s.Id == studentId).FirstOrDefault();

            if (studentToRemove != null)
            {
                studentToRemove.Adresse = null;
                ctx.Students.Remove(studentToRemove);
            }

            ctx.SaveChanges();
        }
        /*• Modificare date student*/
        /// <summary>
        /// Changes age for a student selected from the introduced Id.
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="age"></param>
        [HttpPut("change-age{studentId}")]
        public void ChangeStudentAges([FromRoute] int studentId, [FromBody] int age)
        {
            using var ctx = new CatalogueDbContext();

            var student = ctx.Students.Where(s => s.Id == studentId).FirstOrDefault();

            student.Age = age;

            ctx.SaveChanges();
        }
        /// <summary>
        /// Changes student's data.
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="studentData"></param>
        [HttpPut("change-data{studentId}")]
        public void ChangeStudentData([FromRoute] int studentId, [FromBody] StudentData studentData)
        {
            using var ctx = new CatalogueDbContext();

            var student = ctx.Students.Where(s => s.Id == studentId).FirstOrDefault();
            if (studentData.FirstName!="string")
            {
                student.FirstName = studentData.FirstName;
            }
            if ( studentData.LastName != "string")
            {
                student.LastName = studentData.LastName;
            }
            if (studentData.Age>0)
            {
                student.Age = studentData.Age;
            }
            
            ctx.SaveChanges();
        }
        /*• Modificare adresa student
     • In cazul in care studentul nu are adresa, aceasta va fi creeata*/
        /// <summary>
        /// Adds or changes student adresse.
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="adresse"></param>
        [HttpPut("change-Adresse{studentId}")]
        public void ChangeStudentAdresse([FromRoute] int studentId, [FromBody] AddressToCreate adresse)
        {
            using var ctx = new CatalogueDbContext();

            var student = ctx.Students.Include(s => s.Adresse).Where(s => s.Id == studentId).FirstOrDefault();

            if (student.Adresse == null)
            {
                student.Adresse = new Address
                {
                    City = adresse.City,
                    Street = adresse.Street,
                    Number = adresse.Number,
                };
            }
            else
            {
                var adresseToChange = student.Adresse;
                adresseToChange.City = adresse.City;
                adresseToChange.Street = adresse.Street;
                adresseToChange.Number = adresse.Number;
            }

            ctx.SaveChanges();
        }
        /*• Stergerea unui student
     • Cu un parametru care va specifica daca adresa este la randul ei
stearsa*/
        /// <summary>
        /// Deletes stundent with or without adresse.
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="deleteAdresse"></param>
        [HttpDelete("delete")]
        public void DeleteStudentAndAdresse([FromBody] int studentId, [FromQuery] bool deleteAdresse)
        {
            using var ctx = new CatalogueDbContext();
            var studentToRemove = ctx.Students.Include(a => a.Adresse).Where(s => s.Id == studentId).FirstOrDefault();

            if (studentToRemove != null)
            {
                if (deleteAdresse == true)
                {
                    var adresseToRemove = ctx.Adresses.Where(s => s.StudentId == studentId).FirstOrDefault();
                    ctx.Adresses.Remove(adresseToRemove);
                    ctx.Students.Remove(studentToRemove);
                }
                else
                {
                    ctx.Students.Remove(studentToRemove);
                }
            }

            ctx.SaveChanges();
        }
    }
}
