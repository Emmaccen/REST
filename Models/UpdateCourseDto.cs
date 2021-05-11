using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CourseLibrary.API.Models
{

    public class UpdateCourseDto : CourseForManipulationValidation //: IValidatableObject 
    {
        [Required]
        public override string Description { get => base.Description; set => base.Description = value; }

        /*public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Title == Description)
                yield return new ValidationResult("Title and Description cannot be the same",
                    new[] { "CreateCoursesDto" });
        }*/
    }
}
