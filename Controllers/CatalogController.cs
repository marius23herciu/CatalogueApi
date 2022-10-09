using CatalogApi.DTOs;
using CatalogApi.Extensions;
using laborator19_Catalog_;
using laborator19_Catalog_.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatalogApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly CatalogueDbContext ctx;
        private readonly DataLayer dataLayer;
        public CatalogController(CatalogueDbContext ctx, DataLayer dataLayer)
        {
            this.ctx = ctx;
            this.dataLayer = dataLayer;
        }


        /* • Adaugarea unui curs*/

        /// <summary>
        /// Adds a subject to catalog.
        /// </summary>
        /// <param name="subjectToCreate"></param>
        /// <returns></returns>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SubjectToGet))]
        public IActionResult AddSubject([FromBody] SubjectToCreate subjectToCreate)
        {
            var newSubject = dataLayer.AddSubject(subjectToCreate.Name).ToDto();
            return Created("New Subject Created.", newSubject);
        }

        /* Acordarea unei note unui student
                • La adaugarea notei serverul va complete automat data si ora
                acordarii ca fiind data si ora curenta*/
        /// <summary>
        /// Adds mark to a student. Put as parameters Id of student and Id of subject for the mark.
        /// </summary>
        /// <param name="markToCreate"></param>
        /// <returns></returns>
        [HttpPut("give-mark{studentId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public IActionResult AddMarkToStudent([FromBody] MarkToCreate markToCreate)
        {
            var student = ctx.Students.Where(s => s.Id == markToCreate.StudentId).FirstOrDefault();

            if (student == null)
            {
                return NotFound($"Student with Id {markToCreate.StudentId} does not exist.");
            }

            var subject = ctx.Subjects.Where(s => s.Id == markToCreate.SubjectId).FirstOrDefault();

            if (subject == null)
            {
                return NotFound($"Subject with Id {markToCreate.SubjectId} does not exist.");
            }

            var newMark = dataLayer.AddMarkToStudent(markToCreate.StudentId, markToCreate.SubjectId, markToCreate.Value).ToDto();
            return Ok($"{newMark.Value} was added in {subject.ToDto().Name} at {newMark.DateTime} for {student.ToDto().FirstName} {student.ToDto().LastName}.");
        }

        /* • Obtinerea tuturor notelor unui student. */
        /// <summary>
        /// Returns all marks for selected student.
        /// </summary>
        /// <param name="id">Id of selected student.</param>
        /// <returns></returns>
        [HttpGet("all-marks-for-{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<int>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public IActionResult GetAllMarks([FromRoute] int id)
        {
            var student = ctx.Students.Include(m => m.Marks).Where(s => s.Id == id).FirstOrDefault();
            if (student == null)
            {
                return NotFound($"{student.FirstName} {student.LastName} does not exist.");
            }

            var marks = student.Marks.Select(v => v.Value).ToList();
            if (marks.Count == 0)
            {
                return NotFound($"{student.FirstName} {student.LastName} has no marks.");
            }

            return Ok(dataLayer.GetAllMarks(id));
        }

        /*• Obtinerea notelor unui student pentru un anumit curs*/
        /// <summary>
        /// Returns all the marks for a selected student and subject.
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        [HttpGet("all-marks-for-{studentId}/in-{subjectId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<int>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public IActionResult GetAllMarksForSpecificSubject([FromRoute] int studentId, [FromRoute] int subjectId)
        {
            var student = ctx.Students.Include(m => m.Marks).Where(s => s.Id == studentId).FirstOrDefault();
            if (student == null)
            {
                return NotFound($"{student.FirstName} {student.LastName} does not exist.");
            }

            var subject = ctx.Subjects.Where(s => s.Id == subjectId).FirstOrDefault();
            if (subject == null)
            {
                return NotFound($"Subject does not exist.");
            }

            var marks = dataLayer.GetAllMarksForSpecificSubject(studentId, subjectId);
            return Ok(marks);
        }

        /*• Obtinerea mediilor per materie ale unui student*/
        /// <summary>
        /// Returns a list of all averages for a selected student.
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns></returns>
        [HttpGet("all-average-for-{studentId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<double>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public IActionResult GetAllAveragesForOneStudent([FromRoute] int studentId)
        {
            var student = ctx.Students.Include(s => s.Subjects).Include(m => m.Marks).Where(s => s.Id == studentId).FirstOrDefault();
            if (student == null)
            {
                return NotFound($"{student.FirstName} {student.LastName} does not exist.");
            }

            var marks = student.Marks.Select(v => v.Value).ToList();
            if (marks.Count == 0)
            {
                return NotFound("Sutdent has no marks.");
            }

            var allAverages = dataLayer.GetAllAveragesForOneStudent(studentId);

            return Ok(allAverages);
        }

        /* Obtinerea listei studentilor in ordine a mediilor
                • Ordinea este configurabila ascending/descending
                • DTO-ul va returna informatiile despre student, fara adresa, note,
                dar cu media generala calculate
        */

        [HttpGet("all-averages")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<double>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public IActionResult GetAllAveragesInOrder([FromQuery] bool ascendingOrder)
        {
            var students = ctx.Students.Include(s => s.Subjects).Include(m => m.Marks).ToList();
            if (students == null)
            {
                return NotFound($"There are no students in catalog.");
            }

            var subjects = ctx.Subjects.ToList();
            if (subjects == null)
            {
                return NotFound($"There are no subjects in catalog.");
            }

            var orderedAverages = dataLayer.GetAllAveragesInOrder(ascendingOrder);

            return Ok(orderedAverages);
        }

    }
}
