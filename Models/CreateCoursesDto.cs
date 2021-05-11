using CourseLibrary.API.ValidationAttributes;

namespace CourseLibrary.API.Models
{
    [CourseTitleMustBeDifferentFromDescriptionAttribute(ErrorMessage = "You can't have Name and Desc with same value")]
    public class CreateCoursesDto : CourseForManipulationValidation //: IValidatableObject
    {
    }
}
