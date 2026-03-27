using FluentValidation;

namespace MasterNet.Application.Courses.CourseCreate;

public class CourseCreateValidator : AbstractValidator<CourseCreateRequest>
{
    public CourseCreateValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("The title is required");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("The description is required");
    }
}