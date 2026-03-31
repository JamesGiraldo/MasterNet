using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MasterNet.Application.Abstractions;
using MasterNet.Application.Core;
using MasterNet.Application.Courses.CourseGet;
using MasterNet.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MasterNet.Application.Courses.CoursesGet;

public class CoursesGetQuery
{

    public record CoursesGetQueryRequest : IRequest<Result<PagedList<CourseResponse>>>
    {
        public GetCoursesRequest? CoursesRequest { get; set; }
    }

    internal class CoursesGetQueryHandler
    : IRequestHandler<CoursesGetQueryRequest, Result<PagedList<CourseResponse>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CoursesGetQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<PagedList<CourseResponse>>> Handle(
            CoursesGetQueryRequest request,
            CancellationToken cancellationToken
        )
        {

            IQueryable<Course> queryable = _context.Courses!
                                            .Include(x => x.Instructors)
                                            .Include(x => x.Qualifications)
                                            .Include(x => x.Prices);

            var predicate = ExpressionBuilder.New<Course>();
            if (!string.IsNullOrEmpty(request.CoursesRequest!.Title))
            {
                predicate = predicate
                    .And(y => y.Title!.ToLower()
                    .Contains(request.CoursesRequest.Title.ToLower()));
            }


            if (!string.IsNullOrEmpty(request.CoursesRequest!.Description))
            {
                predicate = predicate
                .And(y => y.Description!.ToLower()
                .Contains(request.CoursesRequest.Description.ToLower()));
            }

            if (!string.IsNullOrEmpty(request.CoursesRequest!.OrderBy))
            {
                Expression<Func<Course, object>>? orderBySelector =
                                request.CoursesRequest.OrderBy!.ToLower() switch
                                {
                                    "title" => course => course.Title!,
                                    "description" => course => course.Description!,
                                    _ => course => course.Title!
                                };

                bool orderBy = request.CoursesRequest.OrderAsc.HasValue
                            ? request.CoursesRequest.OrderAsc.Value
                            : true;

                queryable = orderBy
                            ? queryable.OrderBy(orderBySelector)
                            : queryable.OrderByDescending(orderBySelector);
            }

            queryable = queryable.Where(predicate);

            var cursosQuery = queryable
            .ProjectTo<CourseResponse>(_mapper.ConfigurationProvider)
            .AsQueryable();

            var pagination = await PagedList<CourseResponse>.CreateAsync(
                cursosQuery,
                request.CoursesRequest.PageNumber,
                request.CoursesRequest.PageSize
            );

            return Result<PagedList<CourseResponse>>.Success(pagination);
        }
    }

}