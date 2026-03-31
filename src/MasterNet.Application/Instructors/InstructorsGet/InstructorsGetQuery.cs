using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MasterNet.Application.Abstractions;
using MasterNet.Application.Core;
using MasterNet.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MasterNet.Application.Instructors.InstructorsGet;

public class InstructorsGetQuery : IRequest<Result<PagedList<InstructorResponse>>>
{
    public record InstructorsGetQueryRequest : IRequest<Result<PagedList<InstructorResponse>>>
    {
        public InstructorsGetRequest? InstructorsRequest { get; set; }
    }

    internal class InstructorsGetQueryHandler
        : IRequestHandler<InstructorsGetQueryRequest,
        Result<PagedList<InstructorResponse>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public InstructorsGetQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<PagedList<InstructorResponse>>> Handle(
            InstructorsGetQueryRequest request,
            CancellationToken cancellationToken
        )
        {
            IQueryable<Instructor> queryable = _context.Instructors!
                                            .Include(x => x.Courses)
                                            .Include(x => x.CourseInstructors);

            var predicate = ExpressionBuilder.New<Instructor>();
            if (!string.IsNullOrEmpty(request.InstructorsRequest!.Name))
            {
                predicate = predicate
                    .And(x => x.Name!.ToLower()
                    .Contains(request.InstructorsRequest.Name.ToLower()));
            }

            if (!string.IsNullOrEmpty(request.InstructorsRequest!.LastName))
            {
                predicate = predicate
                    .And(x => x.LastName!.ToLower()
                    .Contains(request.InstructorsRequest.LastName.ToLower()));
            }

            if (!string.IsNullOrEmpty(request.InstructorsRequest!.Degree))
            {
                predicate = predicate
                    .And(x => x.Degree!.ToLower()
                    .Contains(request.InstructorsRequest.Degree.ToLower()));
            }

            if (!string.IsNullOrEmpty(request.InstructorsRequest!.OrderBy))
            {
                Expression<Func<Instructor, object>>? orderBySelector =
                request.InstructorsRequest.OrderBy!.ToLower() switch
                {
                    "name" => instructor => instructor.Name!,
                    "lastname" => instructor => instructor.LastName!,
                    "degree" => instructor => instructor.Degree!,
                    _ => instructor => instructor.CreatedAt
                };

                bool orderBy = request.InstructorsRequest.OrderAsc.HasValue
                            ? request.InstructorsRequest.OrderAsc.Value
                            : true;

                queryable = orderBy
                            ? queryable.OrderBy(orderBySelector)
                            : queryable.OrderByDescending(orderBySelector);
            }

            queryable = queryable.Where(predicate);

            var instructorsQuery = queryable
            .ProjectTo<InstructorResponse>(_mapper.ConfigurationProvider)
            .AsQueryable();

            var pagination = await PagedList<InstructorResponse>.CreateAsync(
                instructorsQuery,
                request.InstructorsRequest.PageNumber,
                request.InstructorsRequest.PageSize
            );

            return Result<PagedList<InstructorResponse>>.Success(pagination);
        }
    }
}

public class InstructorResponse
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public string? LastName { get; set; }
    public string? Degree { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}