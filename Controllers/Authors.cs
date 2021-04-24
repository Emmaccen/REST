using AutoMapper;
using CourseLibrary.API.DbContexts;
using CourseLibrary.API.Models;
using CourseLibrary.API.ResourceParams;
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
            courseLib.AddAuthor(newAuthor);
            var response = mapper.Map<Models.AuthorDto>(newAuthor);
            courseLib.Save();
            return CreatedAtRoute("authorRoute", new { authorId = response.Id}, response);
        }



        [HttpGet]
        [HttpHead]
        public ActionResult<IEnumerable<AuthorDto>> GetAuthors( [FromQuery] AuthorsParams queries)
        {
            var databaseResult = courseLib.GetAuthors(queries);

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

    }
}
