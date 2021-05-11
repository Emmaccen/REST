using AutoMapper;
using CourseLibrary.API.DbContexts;
using CourseLibrary.API.Models;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CourseLibrary.API.Controllers
{
    [ApiController]
    [Route("Api/authors/{authorId}/courses")]
    public class Courses : ControllerBase
    {
        private readonly CourseLibraryContext courseLibrary;
        private readonly ICourseLibraryRepository courseLibraryRepository;
        private readonly IMapper mapper;

        public Courses(CourseLibraryContext courseLibrary,
            ICourseLibraryRepository courseLibraryRepository,
            IMapper mapper)
        {
            this.courseLibrary = courseLibrary;
            this.courseLibraryRepository = courseLibraryRepository;
            this.mapper = mapper;
        }

        [HttpGet(Name = "getCourses")]
        public ActionResult<IEnumerable<CoursesDto>> GetCourses(Guid authorId)
        {
            if (!courseLibraryRepository.AuthorExists(authorId))
                return NotFound();

            var databaseResult = courseLibraryRepository.GetCourses(authorId);
            return Ok(mapper.Map<IEnumerable<CoursesDto>>(databaseResult));
        }

        [HttpGet("{courseId}", Name = "getCourse")]
        public ActionResult<CoursesDto> GetCourse(Guid authorId, Guid courseId)
        {
            if (!courseLibraryRepository.AuthorExists(authorId))
                return NotFound();
            var databaseResult = courseLibraryRepository.GetCourse(authorId, courseId);

            if (databaseResult == null)
                return NotFound();

            return Ok(mapper.Map<CoursesDto>(databaseResult));
        }

        [HttpPost]
        public ActionResult<IEnumerable<CreateCoursesDto>> CreateCourse(Guid authorId, IEnumerable<CreateCoursesDto> course)
        {
            Console.WriteLine(course.GetType());
            if (!courseLibraryRepository.AuthorExists(authorId))
                return NotFound();
            if (course.Count() == 0)
            {
                ModelState.AddModelError("Empty Course", "You cannot create an empty course");
                return BadRequest(ModelState);
            }

            var newCourse = mapper.Map<IEnumerable<Entities.Course>>(course);
            courseLibraryRepository.AddCourse(authorId, newCourse);
            courseLibraryRepository.Save();
            return CreatedAtRoute("getCourses",
                new { authorId = authorId },
                mapper.Map<IEnumerable<Models.CreateCoursesDto>>(newCourse));
        }

        [HttpPut("{courseId}")]
        public ActionResult<UpdateCourseDto> UpdateCourseForAuthor(
            Guid authorId, Guid courseId, UpdateCourseDto courseUpdate)
        {
            if (courseUpdate.Title == courseUpdate.Description) { }
            ModelState.AddModelError("Title And Description", "Name and Description cannot be the same");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!courseLibraryRepository.AuthorExists(authorId))
                return NotFound();
            var databaseResult = courseLibraryRepository.GetCourse(authorId, courseId);

            if (databaseResult == null)
                return NotFound();

            var update = mapper.Map(courseUpdate, databaseResult);
            courseLibraryRepository.UpdateCourse(update);
            courseLibraryRepository.Save();

            return CreatedAtRoute("getCourse",
                new { authorId = update.AuthorId, courseId = update.Id }
                , mapper.Map<UpdateCourseDto>(update));
        }

       /* [HttpPatch]
        public ActionResult<UpdateCourseDto> UpdateCourseForAuthor(
            Guid authorId, Guid courseId, JsonPatchDocument<UpdateCourseDto> courseUpdate)
        {
            if (courseUpdate.Title == courseUpdate.Description) { }
            ModelState.AddModelError("Title And Description", "Name and Description cannot be the same");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!courseLibraryRepository.AuthorExists(authorId))
                return NotFound();

            var databaseResult = courseLibraryRepository.GetCourse(authorId, courseId);

            if (databaseResult == null)
                return NotFound();

            var courseToPatch = mapper.Map<UpdateAuthorDto>(databaseResult);

            courseUpdate.ApplyTo(courseToPatch, ModelState);

            if (!ModelState.IsValid || !TryValidateModel(courseToPatch))
                return BadRequest();

            var update = mapper.Map(courseToPatch, databaseResult);

            courseLibraryRepository.UpdateCourse(databaseResult);

            courseLibraryRepository.Save();

            return CreatedAtRoute("getCourse",
                new { authorId = update.AuthorId, courseId = update.Id }
                , mapper.Map<UpdateCourseDto>(update));
        }*/


        // DELETING

        [HttpDelete("{courseId}")]
        public ActionResult DeleteCourse(Guid authorId, Guid courseId)
        {
            if (!courseLibraryRepository.AuthorExists(authorId))
                return NotFound();

            var databaseResult = courseLibraryRepository.GetCourse(authorId, courseId);

            if (databaseResult == null)
                return NotFound();

            courseLibraryRepository.DeleteCourse(databaseResult);

            courseLibraryRepository.Save();

            return NoContent();
        }
    }
}
