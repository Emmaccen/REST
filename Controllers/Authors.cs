using AutoMapper;
using CourseLibrary.API.DbContexts;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.Models;
using CourseLibrary.API.ResourceParams;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

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
            return CreatedAtRoute("authorRoute", new { authorId = response.Id }, response);
        }

        [HttpPut("{authorId}")]
        public ActionResult UpdateAuthor(Guid authorId, [FromBody] UpdateAuthorDto updateAuthorDto)
        {
            if (!courseLib.AuthorExists(authorId))
                return NotFound();

            var update = mapper.Map(courseLib.GetAuthor(authorId), updateAuthorDto);
            //courseLib.UpdateAuthor(update);
            courseLib.Save();
            return NoContent();

        }

        [HttpGet("authorsCollection/{ids}", Name = "getAuthorCollections")]
        public ActionResult GetAuthorsCollectionWithIds(
            [FromRoute]
            [ModelBinder(BinderType = typeof(ArrayModelBinder))]
            IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            var authorsEntities = courseLib.GetAuthors(ids);

            if (ids.Count() != authorsEntities.Count())
                return NotFound();

            return Ok(mapper.Map<IEnumerable<AuthorDto>>(authorsEntities));
        }

        [HttpPost("authorCollections")]
        public ActionResult CreateAuthor([FromBody] IEnumerable<CreateAuthorDto> author)
        {
            var newAuthor = mapper.Map<IEnumerable<Entities.Author>>(author);
            foreach (var a in newAuthor)
            {
                courseLib.AddAuthor(a);
            }

            courseLib.Save();
            var response = mapper.Map<IEnumerable<Models.AuthorDto>>(newAuthor);
            var idsAsString = string.Join(",", response.Select(id => id.Id));

            return CreatedAtRoute("getAuthorCollections", new { ids = idsAsString }, response);
        }

        [HttpGet(Name = "getAllAuthors")]
        [HttpHead]
        public ActionResult<IEnumerable<AuthorDto>> GetAuthors([FromQuery] AuthorsParams queries)
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

        [HttpOptions]
        public IActionResult GetAuthorsOptions()
        {
            Response.Headers.Add("Allow", "GET, POST, DELETE, OPTIONS");
            return Ok();
        }
    }
}
