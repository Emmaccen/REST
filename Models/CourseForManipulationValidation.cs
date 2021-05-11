using System.ComponentModel.DataAnnotations;

namespace CourseLibrary.API.Models
{
    //[CourseTitleMustBeDifferentFromDescriptionAttribute(ErrorMessage = "You can't have Name and Desc with same value")]
    public abstract class CourseForManipulationValidation
    {
        [Required(ErrorMessage = "You must provide a value")]
        [MaxLength(100, ErrorMessage = "Only values less than or equal to 100 characters are accepted")]
        public string Title { get; set; }

        [MaxLength(1500, ErrorMessage = "Only values less than or equal to 1500 characters are accepted")]
        public virtual string Description { get; set; }
    }
}
