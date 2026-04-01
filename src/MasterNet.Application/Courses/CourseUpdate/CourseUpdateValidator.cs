using FluentValidation;

namespace MasterNet.Application.Courses.CourseUpdate;

public class CourseUpdateValidator : AbstractValidator<CourseUpdateRequest>
{
    public CourseUpdateValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("The id is required and must be a valid GUID");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("The title is required and must be a valid string");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("The description is required and must be a valid string");
    }
}