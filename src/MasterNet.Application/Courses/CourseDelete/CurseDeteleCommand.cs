using FluentValidation;
using MasterNet.Application.Abstractions;
using MasterNet.Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MasterNet.Application.Courses.CourseDelete;

public class CurseDeteleCommand
{
    public record CourseDeleteCommandRequest(Guid CourseId) : IRequest<Result<Unit>>;

    internal class CourseDeleteCommandHandler : IRequestHandler<CourseDeleteCommandRequest, Result<Unit>>
    {
        private readonly IApplicationDbContext _context;

        public CourseDeleteCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Unit>> Handle(
            CourseDeleteCommandRequest request,
            CancellationToken cancellationToken
        )
        {
            var course = await _context.Courses.FirstOrDefaultAsync(x => x.Id == request.CourseId);
            if (course is null) return Result<Unit>.Failure("Course Not Found");

            _context.Courses.Remove(course);

            var result = await _context.SaveChangesAsync(cancellationToken) > 0;

            return result
                ? Result<Unit>.Success(Unit.Value)
                : Result<Unit>.Failure("Failed to delete course");
        }
    }

    public class CourseDeleteCommandRequestValidator
        : AbstractValidator<CourseDeleteCommandRequest>
    {
        public CourseDeleteCommandRequestValidator()
        {
            RuleFor(x => x.CourseId)
                .NotEmpty()
                .WithMessage("Course Id is required and must be a valid GUID");
        }
    }
}