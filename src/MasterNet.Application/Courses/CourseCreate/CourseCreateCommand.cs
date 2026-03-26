using MediatR;
using MasterNet.Application.Abstractions;
using MasterNet.Domain.Entities;

namespace MasterNet.Application.Courses.CourseCreate;

public class CourseCreateCommand
{

    public record CourseCreateCommandRequest(CourseCreateRequest courseCreateRequest)
        : IRequest<Guid>;

    internal class CourseCreateCommandHandler
        : IRequestHandler<CourseCreateCommandRequest, Guid>
    {

        private readonly IApplicationDbContext _context;

        public CourseCreateCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(
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
            await _context.SaveChangesAsync(cancellationToken);
            return course.Id;
        }
    }
}