using FluentValidation;
using MasterNet.Application.Abstractions;
using MasterNet.Application.Core;
using MasterNet.Application.Interfaces;
using MasterNet.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

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

            if (request.courseCreateRequest.InstructorId != null)
            {
                var instructor = _context.Instructors.FirstOrDefault(x => x.Id == request.courseCreateRequest.InstructorId);
                if (instructor is null) return Result<Guid>.Failure("Instructor not found");

                course.Instructors = new List<Instructor> { instructor };
            }

            if (request.courseCreateRequest.PriceId != null)
            {
                var price = await _context.Prices.FirstOrDefaultAsync(x => x.Id == request.courseCreateRequest.PriceId);
                if (price is null) return Result<Guid>.Failure("Price not found");

                course.Prices = new List<Price> { price };
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