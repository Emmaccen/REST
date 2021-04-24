using AutoMapper;
using CourseLibrary.API.DbContexts;
using CourseLibrary.API.Models;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Controllers
{
    [ApiController]
    [Route("Api/[controller]")]
    public class Authors : ControllerBase
    {
        private readonly CourseLibraryContext courseLibrary;
        private readonly ICourseLibraryRepository courseLib;
        private readonly IMapper mapper;

        public Authors(CourseLibraryContext courseLibrary, 
            ICourseLibraryRepository courseLibraryRepository, 
            IMapper mapper)
        {
            this.courseLibrary = courseLibrary;
            this.courseLib = courseLibraryRepository;
            this.mapper = mapper;
        }

        public CourseLibraryRepository CourseLibraryRepository { get; }

        [HttpPost]
        public ActionResult CreateAuthor([FromBody] CreateAuthorDto author)
        {
            var newAuthor = mapper.Map<Entities.Author>(author);
            courseLibrary.Authors.Add(newAuthor);
            var response = mapper.Map<Models.AuthorDto>(newAuthor);
            courseLib.Save();
            return Ok(response);
        }

        [HttpGet]
        public ActionResult<IEnumerable<AuthorDto>> GetAuthors()
        {
            var databaseResult = courseLib.GetAuthors();

            return Ok(mapper.Map<IEnumerable<AuthorDto>>(databaseResult));
        }
        [HttpGet("{authorId}", Name = "authorRoute")]
        public ActionResult<AuthorDto> GetAuthor(Guid authorId)
        {
            var databaseResult = courseLib.GetAuthor(authorId);
            if (databaseResult == null)
                return NotFound();
            return Ok(mapper.Map<AuthorDto>(databaseResult));
        }
        [HttpGet("{authorId}/courses")]
        public ActionResult<IEnumerable<CoursesDto>> GetCourses(Guid authorId)
        {
            if (!courseLib.AuthorExists(authorId))
                return NotFound();
            var databaseResult = courseLib.GetCourses(authorId);
            return Ok(mapper.Map<IEnumerable<CoursesDto>>(databaseResult));
        }
        [HttpGet("{authorId}/courses/{courseId}")]
        public ActionResult<CoursesDto> GetCourse(Guid authorId, Guid courseId)
        {
            if (!courseLib.AuthorExists(authorId))
                return NotFound();
            var databaseResult = courseLib.GetCourse(authorId, courseId);
            if (databaseResult == null)
                return NotFound();
            return Ok(mapper.Map<CoursesDto>(databaseResult));
        }
    }
}
