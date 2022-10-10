using CatalogApi.DTOs;
using CatalogApi.Extensions;
using laborator19_Catalog_;
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
        private readonly CatalogueDbContext ctx;
        private readonly DataLayer dataLayer;
        public TeacherController(CatalogueDbContext ctx, DataLayer dataLayer)
        {
            this.ctx = ctx;
            this.dataLayer = dataLayer;
        }

        /*• Adaugarea unui teacher*/

        /// <summary>
        /// Adds a teacher to catalog.
        /// </summary>
        /// <param name="teacherToCreate"></param>
        /// <returns></returns>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(TeacherToGet))]
        public IActionResult CreateTeacher([FromBody] TeacherToCreate teacherToCreate)
        {
            var newTeacher = dataLayer.CreateTeacher(teacherToCreate.FirstName, teacherToCreate.LastName, teacherToCreate.Rank);

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
            var teacherToDelete = ctx.Teachers.Include(a=>a.Address).Where(s => s.Id == teacherId).FirstOrDefault();

            if (teacherToDelete == null)
            {
                return NotFound("Teacher not found.");
            }

            dataLayer.DeleteTeacher(teacherId);

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
            var teacher = ctx.Teachers.Include(s => s.Address).Where(t => t.Id == teacherId).FirstOrDefault();

            if (teacher == null)
            {
                return NotFound($"Teacher with Id {teacherId} does not exist.");
            }

            dataLayer.ChangeTeachersAddress(teacherId, address.City, address.Street, address.Number);

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
            var teacher = ctx.Teachers.Where(t => t.Id == teacherId).Include(s => s.Subject).FirstOrDefault();

            if (teacher == null)
            {
                return NotFound($"Teacher with Id {teacherId} does not exist.");
            }
            if (teacher.Subject!=null)
            {
                return BadRequest($"Teacher with Id {teacherId} is appointed to another subject.");
            }
            foreach (var teacherInList in ctx.Teachers.Include(s=>s.Subject))
            {
                if (teacherInList.SubjectId==subjectId)
                {
                    return BadRequest($"Subjet is appointed to another teacher.");
                }
            }

            var subjects = ctx.Subjects.Select(s => s.Id).ToList();
            if (!subjects.Contains(subjectId))
            {
                return NotFound($"Subject with Id {subjectId} does not exist.");
            }
            
            dataLayer.GivesCourseToTeacher(teacherId,subjectId);

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
            var teacher = ctx.Teachers.Where(t => t.Id == teacherId).FirstOrDefault();

            if (teacher == null)
            {
                return NotFound($"Teacher with Id {teacherId} does not exist.");
            }

            if (teacher.Rank==Rank.Professor)
            {
                return BadRequest("Teacher allready has the highest rank.");
            }

            dataLayer.PromoteTeacher(teacherId);

            return Ok("Teacher has been promoted.");
        }

        
    }
}
