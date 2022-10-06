using CatalogApi.DTOs;
using CatalogApi.Extensions;
using laborator19_Catalog_.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;

namespace CatalogApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        
        /* Stergerea unui curs
              • Ce alte stergeri implica?*/

        /// <summary>
        /// Deletes a selected subjects. The teacher remains with NULL to SubjectId.
        /// </summary>
        /// <param name="subjectId"></param>
        /// <param name="keepMarks">If true, all marks from the deleted subject remain with NULL to SubjectId.</param>
        /// <returns></returns>
        [HttpDelete("delete-subject{subjectId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public IActionResult DeleteSubject([FromBody] int subjectId, [FromQuery] bool keepMarks)
        {
            using var ctx = new CatalogueDbContext();

            var subjectToDelete = ctx.Subjects.Include(m=>m.Marks).Include(t => t.Teacher).Where(s => s.Id == subjectId).FirstOrDefault();
            if (subjectToDelete == null)
            {
                return NotFound("Subject not found.");
            }

            if (keepMarks!=true)
            {
                var marks = ctx.Marks.Where(m => m.SubjectId == subjectId).ToList();
                ctx.Marks.RemoveRange(marks);
            }
            
            ctx.Subjects.Remove(subjectToDelete);
            ctx.SaveChanges();

            return Ok($"Subject with id {subjectId} was deleted.");
        }

        /*• Adaugarea unui teacher*/

        /// <summary>
        /// Adds a teacher to catalog.
        /// </summary>
        /// <param name="teacherToCreate"></param>
        /// <returns></returns>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(TeacherToGet))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public IActionResult CreateTeacher([FromBody] TeacherToCreate teacherToCreate)
        {
            using var ctx = new CatalogueDbContext();

            var newTeacher = new Teacher
            {
                FirstName = teacherToCreate.FirstName,
                LastName = teacherToCreate.LastName,
                Rank = teacherToCreate.Rank,
            };

            ctx.Add(newTeacher);
            ctx.SaveChanges();

            return Created("New Student Created.", newTeacher.ToDto());
        }

        /* Stergerea unui teacher
               • Cum tratati stergerea?*/

        /// <summary>
        /// Deletes a teacher and its address.
        /// </summary>
        /// <param name="teacherId"></param>
        /// <returns></returns>
        [HttpDelete("delete-teacher-{teacherId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public IActionResult DeleteTeacher([FromBody] int teacherId)
        {
            using var ctx = new CatalogueDbContext();

            var teacherToDelete = ctx.Teachers.Include(a=>a.Address).Where(s => s.Id == teacherId).FirstOrDefault();

            if (teacherToDelete == null)
            {
                return NotFound("Teacher not found.");
            }

            var addressToRemove = ctx.Adresses.Where(t => t.TeacherId== teacherId).FirstOrDefault();
            if (addressToRemove!=null)
            {
                ctx.Adresses.Remove(addressToRemove);
            }
            teacherToDelete.Subject = null;
            ctx.Teachers.Remove(teacherToDelete);
            ctx.SaveChanges();

            return Ok($"Teacher with id {teacherId} was deleted.");
        }

        /*• Schimbarea adresei unui teacher*/

        /// <summary>
        /// Changes or adds address(if doesnt't have) to a teacher.
        /// </summary>
        /// <param name="teacherId"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        [HttpPut("change-address{teacherId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public IActionResult ChangeTeachersAddress([FromRoute] int teacherId, [FromBody] AddressToCreate address)
        {
            using var ctx = new CatalogueDbContext();

            var teacher = ctx.Teachers.Include(s => s.Address).Where(t => t.Id == teacherId).FirstOrDefault();

            if (teacher == null)
            {
                return NotFound($"Teacher with Id {teacherId} does not exist.");
            }

            if (teacher.Address == null)
            {
                teacher.Address = new Address
                {
                    City = address.City,
                    Street = address.Street,
                    Number = address.Number,
                };
            }
            else
            {
                var addressToChange = teacher.Address;
                addressToChange.City = address.City;
                addressToChange.Street = address.Street;
                addressToChange.Number = address.Number;
            }

            ctx.SaveChanges();

            return Ok("Teacher's address changed.");
        }

        /*• Alocarea unui curs pentru un teacher
            */

        /// <summary>
        /// Gives a subject to a teacher who doesn't have one.
        /// </summary>
        /// <param name="teacherId"></param>
        /// <param name="subjectId">Id of subject to give.</param>
        /// <returns></returns>
        [HttpPut("gives-subject-to-teacher-{teacherId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        public IActionResult GivesCourseToTeacher([FromRoute] int teacherId, [FromBody] int subjectId)
        {
            using var ctx = new CatalogueDbContext();

            var teacher = ctx.Teachers.Where(t => t.Id == teacherId).Include(s => s.Subject).FirstOrDefault();

            if (teacher == null)
            {
                return NotFound($"Teacher with Id {teacherId} does not exist.");
            }
            if (teacher.Subject!=null)
            {
                return BadRequest($"Teacher with Id {teacherId} is appointed to another subject.");
            }
            var subjects = ctx.Subjects.Select(s => s.Id).ToList();
            if (!subjects.Contains(subjectId))
            {
                return NotFound($"Subject with Id {subjectId} does not exist.");
            }
            var subject= ctx.Subjects.Where(s=>s.Id==subjectId).FirstOrDefault();
            teacher.Subject= subject;
            ctx.SaveChanges();

            return Ok($"Subject with Id {subjectId} has been asigned to teacher with Id {teacherId}.");
        }


        /* Promovarea teacher-ului
                • Instructor devine assistant professor
                • Assistant professor devine associate professor
                • Samd*/

        /// <summary>
        /// Promotes a teacher to next rank.
        /// </summary>
        /// <param name="teacherId"></param>
        /// <returns></returns>
        [HttpPut("promote-teacher{teacherId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        public IActionResult PromoteTeacher([FromRoute] int teacherId )
        {
            using var ctx = new CatalogueDbContext();

            var teacher = ctx.Teachers.Where(t => t.Id == teacherId).FirstOrDefault();

            if (teacher == null)
            {
                return NotFound($"Teacher with Id {teacherId} does not exist.");
            }

            if (teacher.Rank==Rank.Professor)
            {
                return BadRequest("Teacher allready has the highest rank.");
            }

            teacher.Rank++;
            ctx.SaveChanges();

            return Ok("Teacher has been promoted.");
        }

        /*
         Obtinerea tuturor notelor acordate de catre un
         teacher:
               • Va returna o lista ce va contine DTO-uri continand
               valoarea notei, data acordarii precum si id-ul
               studentului
         */

        [HttpGet("all-marks-from-{teacherId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<MarkToGet>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public IActionResult GetAllMarks([FromRoute] int teacherId)
        {
            using var ctx = new CatalogueDbContext();

            var teacher = ctx.Teachers.Include(m => m.Subject).Where(s => s.Id == teacherId).FirstOrDefault();
            if (teacher == null)
            {
                return NotFound($"Teacher does not exist.");
            }

            var marks = ctx.Marks.Where(m => m.SubjectId == teacher.SubjectId).ToList();
            
            if (marks.Count == 0)
            {
                return NotFound($"{teacher.ToDto().FirstName} {teacher.ToDto().LastName} has given no marks.");
            }

            List<MarkToGet> marksList = new List<MarkToGet>();
            foreach (var mark in marks)
            {
                marksList.Add(
                    new MarkToGet
                    {
                        Id= mark.Id,
                        Value = mark.Value,
                        SubjectId= mark.SubjectId,
                        DateTime = mark.DateTime,
                        StudentId = mark.StudentId,
                    });
            }


            return Ok(marksList);
        }

    }
}
