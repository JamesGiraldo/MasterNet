using MediatR;
using MasterNet.Application.Core;
using MasterNet.Application.Instructors.InstructorsGet;
using MasterNet.Application.Prices.PricesGet;
using MasterNet.Application.Qualifications.QualificationsGet;
using MasterNet.Application.Photos.PhotosGet;
using MasterNet.Application.Abstractions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;

namespace MasterNet.Application.Courses.CourseGet;

public class CourseGetQuery
{
    public record CourseGetQueryRequest : IRequest<Result<CourseResponse>>
    {
        public Guid Id { get; set; }
    }

    internal class GetCourseQueryHandler : IRequestHandler<CourseGetQueryRequest, Result<CourseResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetCourseQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<CourseResponse>> Handle(
            CourseGetQueryRequest request,
            CancellationToken cancellationToken
        )
        {
            var course = await _context.Courses.Where(x => x.Id == request.Id)
                    .Include(x => x.Instructors)
                    .Include(x => x.Prices)
                    .Include(x => x.Qualifications)
                    .Include(x => x.Photos)
                    .ProjectTo<CourseResponse>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync();

            if (course == null)
            {
                return Result<CourseResponse>.Failure("Course not found");
            }

            return Result<CourseResponse>.Success(course);
        }
    }

}

public record CourseResponse(
    Guid Id,
    string Title,
    string Description,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    List<InstructorResponse> Instructors,
    List<PriceResponse> Prices,
    List<QualificationResponse> Qualifications,
    List<PhotoResponse> Photos
);