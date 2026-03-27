using MediatR;
using MasterNet.Application.Abstractions;
using MasterNet.Domain.Entities;
using MasterNet.Application.Core;

namespace MasterNet.Application.Courses.CourseCreate;

public class CourseCreateCommand
{

    public record CourseCreateCommandRequest(CourseCreateRequest courseCreateRequest)
        : IRequest<Result<Guid>>;

    internal class CourseCreateCommandHandler
        : IRequestHandler<CourseCreateCommandRequest, Result<Guid>>
    {

        private readonly IApplicationDbContext _context;

        public CourseCreateCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Guid>> Handle(
            CourseCreateCommandRequest request,
            CancellationToken cancellationToken
        )
        {
            var course = new Course
            {
                Id = Guid.NewGuid(),
                Title = request.courseCreateRequest.Title,
                Description = request.courseCreateRequest.Description,
                CreatedAt = request.courseCreateRequest.CreatedAt ?? DateTime.UtcNow,
                UpdatedAt = request.courseCreateRequest.UpdatedAt,
            };

            _context.Courses.Add(course);
            var result = await _context.SaveChangesAsync(cancellationToken) > 0;

            if (!result)
            {
                return Result<Guid>.Failure("Failed to create course");
            }

            return Result<Guid>.Success(course.Id);
        }
    }
}