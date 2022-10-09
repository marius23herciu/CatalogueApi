using CatalogApi.DTOs;
using CatalogApi.Extensions;
using laborator19_Catalog_.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.Text;

namespace CatalogApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly CatalogueDbContext ctx;

        public CatalogController(CatalogueDbContext ctx)
        {
            this.ctx = ctx;
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
            var newSubject = new Subject
            {
                Name = subjectToCreate.Name,
            };

            ctx.Add(newSubject);
            foreach (var student in ctx.Students)
            {
                student.Subjects.Add(newSubject);
            }
            ctx.SaveChanges();

            return Created("New Subject Created.", newSubject.ToDto());
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

            student.Marks.Add((new Mark
            {
                Value = markToCreate.Value,
                SubjectId = markToCreate.SubjectId,
                DateTime = DateTime.Now,
            }));

            ctx.SaveChanges();

            return Ok($"{markToCreate.Value} was added in {subject.ToDto().Name} at {markToCreate.DateTime} for {student.ToDto().FirstName} {student.ToDto().LastName}.");
        }

        /* • Obtinerea tuturor notelor unui student. */
        /// <summary>
        /// Returns all marks for selected student.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("all-marks-for-{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
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

            StringBuilder sb = new StringBuilder();
            foreach (var mark in marks)
            {
                sb.Append($"{mark}, ");
            }
            sb.Length--;
            sb.Length--;

            return Ok($"{student.ToDto().FirstName} {student.ToDto().LastName} has the following marks: {sb.ToString()}");
        }

        /*• Obtinerea notelor unui student pentru un anumit curs*/
        /// <summary>
        /// Returns all the marks for a selected student and subject.
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        [HttpGet("all-marks-for-{studentId}/in-{subjectId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
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

            var marks = student.Marks.Where(s => s.SubjectId == subjectId).Select(v=>v.Value).ToList();

            StringBuilder sb = new StringBuilder();
            foreach (var mark in marks)
            {
                sb.Append($"{mark}, ");
            }

            if (sb.Length < 1)
            {
                return NotFound($"{student.FirstName} {student.LastName} has no marks in {subject.Name}.");
            }
            sb.Length--;
            sb.Length--;

            return Ok($"{student.ToDto().FirstName} {student.ToDto().LastName} has the following marks in {subject.ToDto().Name}: {sb.ToString()}");
        }

        /*• Obtinerea mediilor per materie ale unui student*/
        /// <summary>
        /// Returns a list of all averages for a selected student.
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns></returns>
        [HttpGet("all-average-for-{studentId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public IActionResult GetAllAveragesForOneStudent([FromRoute] int studentId)
        {
            var student = ctx.Students.Include(s=>s.Subjects).Include(m => m.Marks).Where(s => s.Id == studentId).FirstOrDefault();
            if (student == null)
            {
                return NotFound($"{student.FirstName} {student.LastName} does not exist.");
            }

            var marks = student.Marks.Select(v => v.Value).ToList();
            if (marks.Count==0)
            {
                return NotFound("Sutdent has no marks.");
            }

            var subjects = student.Subjects.ToList();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            double average = 0;
            double sum = 0;
            int counter = 0;
            foreach (var subject in subjects)
            {
                sb.Append($"Average for {subject.Name} is: ");
                foreach (var mark in student.Marks)
                {
                    if (mark.SubjectId==subject.Id)
                    {
                        sum += mark.Value;
                        counter++;
                    }
                }
                if (sum==0)
                {
                    sb.Append($"no marks for this subject.");
                }
                else
                {
                    average = sum / counter;
                    sb.Append($"{average.ToString()}");
                }
                sb.AppendLine();
                average = 0;
                sum = 0;
                counter = 0;
            }

            return Ok($"Averages for {student.ToDto().FirstName} {student.ToDto().LastName}: {sb.ToString()} ");
        }

        /* Obtinerea listei studentilor in ordine a mediilor
                • Ordinea este configurabila ascending/descending
                • DTO-ul va returna informatiile despre student, fara adresa, note,
                dar cu media generala calculate
        */

        [HttpGet("all-averages")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
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

            double average = 0;
            double sum = 0;
            int counter = 0;

            double sumOfAverages = 0;
            double finalAverage = 0;
            int counterOfAverages = 0;

            Dictionary<StudentToGet, double> dictionary = new Dictionary<StudentToGet, double>();
           
            foreach (var student in students)
            {
                foreach (var subject in subjects)
                {
                    foreach (var mark in student.Marks)
                    {
                        if (subject.Id == mark.SubjectId)
                        {
                            sum += mark.Value;
                            counter++;
                        }
                    }
                    if (sum > 0)
                    {
                        average = sum / counter;
                        sumOfAverages += average;
                        counterOfAverages++;
                    }
                    sum = 0;
                    counter = 0;
                    average = 0;
                }
                if (sumOfAverages > 0)
                {
                    finalAverage = sumOfAverages / counterOfAverages;
                }
                dictionary.Add(student.ToDto(), finalAverage);
                sumOfAverages = 0;
                finalAverage = 0;
                counterOfAverages = 0;
            }

            IOrderedEnumerable<KeyValuePair<StudentToGet, double>> sortedDictionary;
            if (ascendingOrder==true)
            {
                sortedDictionary = dictionary.OrderBy(v => v.Value);
            }
            else
            {
                sortedDictionary = dictionary.OrderByDescending(v => v.Value);
            }

            StringBuilder sb = new StringBuilder();
            foreach (var student in sortedDictionary)
            {
                sb.AppendLine($"Average for {student.Key.FirstName} {student.Key.LastName} is: {student.Value}");
            }
            
            return Ok($"{sb.ToString()} ");
        }

    }
}
