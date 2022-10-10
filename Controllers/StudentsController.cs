using CatalogApi.DTOs;
using CatalogApi.Extensions;
using laborator19_Catalog_;
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
        private readonly DataLayer dataLayer;
        private readonly CatalogueDbContext ctx;

        public StudentsController(CatalogueDbContext ctx, DataLayer dataLayer)
        {
            this.ctx = ctx;
            this.dataLayer = dataLayer;
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
            var students = dataLayer.GetAllStudents().Select(s => s.ToDto());
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
            var student = dataLayer.GetStudentById(id);
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
            var newStudent = dataLayer.CreateStudent(studentToCreate.FirstName, studentToCreate.LastName, studentToCreate.Age);
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

            dataLayer.DeleteStudent(studentId);

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

            dataLayer.ChangeStudentData(studentId, studentData.FirstName, studentData.LastName, studentData.Age);

            return Ok($"Data for the student with Id {studentId} has been changed.");
        }

        /*• Modificare adresa student
     • In cazul in care studentul nu are adresa, aceasta va fi creeata*/

        /// <summary>
        /// Adds or changes student address.
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="address"></param>
        [HttpPut("change-Adresse{studentId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public IActionResult ChangeStudentAddress([FromRoute] int studentId, [FromBody] AddressToCreate address)
        {
            var student = ctx.Students.Include(s => s.Adresse).Where(s => s.Id == studentId).FirstOrDefault();

            if (student == null)
            {
                return NotFound($"Student with Id {studentId} does not exist.");
            }

            dataLayer.ChangeStudentAddress(studentId, address.City, address.Street, address.Number);

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
        public IActionResult DeleteStudentAndAddress([FromBody] int studentId, [FromQuery] bool deleteAddress)
        {
            var studentToRemove = ctx.Students.Include(a => a.Adresse).Where(s => s.Id == studentId).FirstOrDefault();

            if (studentToRemove == null)
            {
                return NotFound($"Student with Id {studentId} does not exist.");
            }

            dataLayer.DeleteStudentAndAddress(studentId, deleteAddress);

            if (deleteAddress == false)
            {
                return Ok("Student deleted without address.");
            }

            return Ok("Student deleted with his address.");
        }
    }
}
