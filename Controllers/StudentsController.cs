using CatalogApi.DTOs;
using CatalogApi.Extensions;
using laborator19_Catalog_.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace CatalogApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {

        private readonly CatalogueDbContext ctx;

        public StudentsController(CatalogueDbContext ctx)
        {
            this.ctx = ctx;
        }

        /*• Obtinerea tuturor studentilor*/

        /// <summary>
        /// Returns all students with name and age.
        /// </summary>
        /// <returns></returns>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<StudentToGet>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public IActionResult GetAllStudents()
        {
            var students = ctx.Students.Include(s=>s.Subjects).Select(s => s.ToDto()).ToList();

            if (students.Count == 0)
            {
                return NotFound("Catalog is empty.");
            }

            return Ok(students);
        }

        /*• Obtinerea unui student dupa ID*/

        /// <summary>
        /// Returns name and age of a selected student by his Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentToGet))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public IActionResult GetStudentById([FromRoute] int id)
        {
            var student = ctx.Students.Where(s => s.Id == id).FirstOrDefault();
            if (student == null)
            {
                return NotFound("Student does not exist.");
            }

            return Ok(student.ToDto());
        }

        /*• Creeare student*/

        /// <summary>
        /// Creates and adds a student in database.
        /// </summary>
        /// <param name="studentToCreate"></param>
        /// <returns></returns>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(StudentToGet))]
        public IActionResult CreateStudent([FromBody] StudentToCreate studentToCreate)
        {
            var subjects = ctx.Subjects.ToList();

            var newStudent = new Student
            {
                FirstName = studentToCreate.FirstName,
                LastName = studentToCreate.LastName,
                Age = studentToCreate.Age,
                Subjects=subjects,
            };

            ctx.Add(newStudent);
            ctx.SaveChanges();

            return Created("New Student Created.", newStudent.ToDto());
        }

        /*• Stergere student*/

        /// <summary>
        /// Deletes a student from Db.
        /// </summary>
        /// <param name="studentId"></param>
        [HttpDelete("delete{studentId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public IActionResult DeleteStudent([FromBody] int studentId)
        {
            var studentToRemove = ctx.Students.Include(a => a.Adresse).Where(s => s.Id == studentId).FirstOrDefault();

            if (studentToRemove == null)
            {
                return NotFound("Student does not exist.");
            }

            studentToRemove.Adresse = null;
            ctx.Students.Remove(studentToRemove);
            ctx.SaveChanges();

            return Ok($"Student with id {studentId} was deleted.");
        }

        /*• Modificare date student*/

        /// <summary>
        /// Changes student's First Name and/or Last Name and/or Age.
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="studentData"></param>
        [HttpPut("change-data{studentId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public IActionResult ChangeStudentData([FromRoute] int studentId, [FromBody] StudentData studentData)
        {
            var student = ctx.Students.Where(s => s.Id == studentId).FirstOrDefault();

            if (student == null)
            {
                return NotFound($"Student with Id {studentId} does not exist.");
            }
            if (studentData.FirstName != string.Empty)
            {
                student.FirstName = studentData.FirstName;
            }
            if (studentData.LastName != string.Empty)
            {
                student.LastName = studentData.LastName;
            }
            if (studentData.Age != null)
            {
                student.Age = studentData.Age;
            }

            ctx.SaveChanges();

            return Ok($"Data for the student with Id {studentId} has been changed.");
        }

        /*• Modificare adresa student
     • In cazul in care studentul nu are adresa, aceasta va fi creeata*/

        /// <summary>
        /// Adds or changes student address.
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="adresse"></param>
        [HttpPut("change-Adresse{studentId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public IActionResult ChangeStudentAdresse([FromRoute] int studentId, [FromBody] AddressToCreate adresse)
        {
            var student = ctx.Students.Include(s => s.Adresse).Where(s => s.Id == studentId).FirstOrDefault();

            if (student == null)
            {
                return NotFound($"Student with Id {studentId} does not exist.");
            }

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

            return Ok("Student's address changed.");
        }

        /*• Stergerea unui student
     • Cu un parametru care va specifica daca adresa este la randul ei
stearsa*/

        /// <summary>
        /// Deletes stundent with or without address.
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="deleteAddress"></param>
        [HttpDelete("delete")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public IActionResult DeleteStudentAndAdresse([FromBody] int studentId, [FromQuery] bool deleteAddress)
        {
            var studentToRemove = ctx.Students.Include(a => a.Adresse).Where(s => s.Id == studentId).FirstOrDefault();

            if (studentToRemove == null)
            {
                return NotFound($"Student with Id {studentId} does not exist.");
            }

            if (studentToRemove != null)
            {
                if (deleteAddress == true)
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

            if (deleteAddress == false)
            {
                return Ok("Student deleted without address.");
            }

            return Ok("Student deleted with his address.");
        }
    }
}
