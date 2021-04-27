using CourseLibrary.API.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Models
{
    [CourseTitleMustBeDifferentFromDescriptionAttribute(ErrorMessage = "You can't have Name and Desc with same value")]
    public class CreateCoursesDto //: IValidatableObject
    {
        [Required(ErrorMessage = "You must provide a value")]
        [MaxLength(100, ErrorMessage = "Only values less than or equal to 100 characters are accepted")]
        public string Title { get; set; }

        [MaxLength(1500, ErrorMessage = "Only values less than or equal to 1500 characters are accepted")]
        public string Description { get; set; }

       /* public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Title == Description)
                yield return new ValidationResult("Title and Description cannot be the same",
                    new[] { "CreateCoursesDto" });
        }*/
    }
}
