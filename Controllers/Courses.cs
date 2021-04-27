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
        public ActionResult<CoursesDto> CreateCourse(Guid authorId, IEnumerable<CreateCoursesDto> course)
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
    }
}
