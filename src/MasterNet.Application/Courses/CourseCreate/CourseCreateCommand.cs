using MediatR;
using MasterNet.Application.Abstractions;
using MasterNet.Domain.Entities;
using MasterNet.Application.Core;
using FluentValidation;
using MasterNet.Application.Interfaces;

namespace MasterNet.Application.Courses.CourseCreate;

public class CourseCreateCommand
{

    public record CourseCreateCommandRequest(CourseCreateRequest courseCreateRequest)
        : IRequest<Result<Guid>>;

    internal class CourseCreateCommandHandler
        : IRequestHandler<CourseCreateCommandRequest, Result<Guid>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPhotoService _photoService;

        public CourseCreateCommandHandler(
            IApplicationDbContext context,
            IPhotoService photoService
        )
        {
            _context = context;
            _photoService = photoService;
        }

        public async Task<Result<Guid>> Handle(
            CourseCreateCommandRequest request,
            CancellationToken cancellationToken
        )
        {
            var courseId = Guid.NewGuid();

            var course = new Course
            {
                Id = courseId,
                Title = request.courseCreateRequest.Title,
                Description = request.courseCreateRequest.Description,
                CreatedAt = request.courseCreateRequest.CreatedAt ?? DateTime.UtcNow,
                UpdatedAt = request.courseCreateRequest.UpdatedAt,
            };

            if (request.courseCreateRequest.Photo != null)
            {
                var photoResult = await _photoService.AddPhoto(request.courseCreateRequest.Photo);

                var photo = new Photo
                {
                    Id = Guid.NewGuid(),
                    PublicId = photoResult.PublicId,
                    Url = photoResult.Url,
                    CourseId = courseId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };

                course.Photos = new List<Photo> { photo };
            }

            _context.Courses.Add(course);
            var result = await _context.SaveChangesAsync(cancellationToken) > 0;

            return result
                ? Result<Guid>.Success(course.Id)
                : Result<Guid>.Failure("Failed to create course");
        }
    }

    public class CourseCreateCommandRequestValidator
        : AbstractValidator<CourseCreateCommandRequest>
    {
        public CourseCreateCommandRequestValidator()
        {
            RuleFor(x => x.courseCreateRequest)
                .SetValidator(new CourseCreateValidator());
        }
    }
}