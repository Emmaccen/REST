using CourseLibrary.API.DbContexts;
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

        public Authors(CourseLibraryContext courseLibrary, ICourseLibraryRepository courseLibraryRepository)
        {
            this.courseLibrary = courseLibrary;
            this.courseLib = courseLibraryRepository;
        }

        public CourseLibraryRepository CourseLibraryRepository { get; }

        [HttpGet]
        public IActionResult GetAuthors()
        {
            var result = courseLib.GetAuthors();
            return Ok(result);
        }
        [HttpGet("{authorId}")]
        public IActionResult GetAuthor(Guid authorId)
        {
            var result = courseLib.GetAuthor(authorId);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
    }
}
