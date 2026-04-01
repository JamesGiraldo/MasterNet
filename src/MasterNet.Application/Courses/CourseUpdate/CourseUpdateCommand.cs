using FluentValidation;
using MasterNet.Application.Abstractions;
using MasterNet.Application.Core;
using MasterNet.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MasterNet.Application.Courses.CourseUpdate;

public class CourseUpdateCommand
{
    public record CourseUpdateCommandRequest(
        CourseUpdateRequest courseUpdateRequest,
        Guid? CourseId
    ) : IRequest<Result<Guid>>;

    internal class CourseUpdateCommandHandler : IRequestHandler<CourseUpdateCommandRequest, Result<Guid>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPhotoService _photoService;

        public CourseUpdateCommandHandler(
            IApplicationDbContext context,
            IPhotoService photoService
        )
        {
            _context = context;
            _photoService = photoService;
        }
        public async Task<Result<Guid>> Handle(
            CourseUpdateCommandRequest request,
            CancellationToken cancellationToken
        )
        {
            var course = await _context.Courses.FirstOrDefaultAsync(x => x.Id == request.CourseId);
            if (course is null)
            {
                return Result<Guid>.Failure("Course Not Found");
            }

            course.Title = request.courseUpdateRequest.Title;
            course.Description = request.courseUpdateRequest.Description;
            course.UpdatedAt = request.courseUpdateRequest.UpdatedAt ?? DateTime.UtcNow;

            var result = await _context.SaveChangesAsync(cancellationToken);

            return result > 0
                ? Result<Guid>.Success(course!.Id)
                : Result<Guid>.Failure("Failed to update course");
        }
    }

    public class CourseUpdateCommandRequestValidator
        : AbstractValidator<CourseUpdateCommandRequest>
    {
        public CourseUpdateCommandRequestValidator()
        {
            RuleFor(x => x.courseUpdateRequest)
                .SetValidator(new CourseUpdateValidator());

            RuleFor(x => x.CourseId)
                .NotNull()
                .NotEmpty()
                .WithMessage("Course Id is required and must be a valid GUID");
        }
    }
}