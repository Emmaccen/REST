using CourseLibrary.API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.ValidationAttributes
{
    public class CourseTitleMustBeDifferentFromDescriptionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, 
            ValidationContext validationContext)
        {
            var course = (IEnumerable<CreateCoursesDto>)validationContext.ObjectInstance;

            foreach (var c in course)
            {
                if (c.Title == c.Description)
                {
                    return new ValidationResult(ErrorMessage,
                        new[] { nameof(CreateCoursesDto) });
                }
            }

            return ValidationResult.Success;
        }
    }
}
