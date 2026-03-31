using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MasterNet.Application.Abstractions;
using MasterNet.Application.Core;
using MasterNet.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MasterNet.Application.Qualifications.QualificationsGet;

public class QualificationsGetQuery : IRequest<Result<PagedList<QualificationResponse>>>
{
    public record QualificationsGetQueryRequest : IRequest<Result<PagedList<QualificationResponse>>>
    {
        public QualificationsGetRequest? QualificationsRequest { get; set; }
    }

    internal class QualificationsGetQueryHandler
    : IRequestHandler<QualificationsGetQueryRequest, Result<PagedList<QualificationResponse>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public QualificationsGetQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<PagedList<QualificationResponse>>> Handle(
            QualificationsGetQueryRequest request,
            CancellationToken cancellationToken
        )
        {
            IQueryable<Qualification> queryable = _context.Qualifications!
                                            .Include(x => x.Course);

            var predicate = ExpressionBuilder.New<Qualification>();

            if (!string.IsNullOrEmpty(request.QualificationsRequest!.Student))
            {
                predicate = predicate
                    .And(x => x.Student!.ToLower()
                    .Contains(request.QualificationsRequest.Student.ToLower()));
            }

            if (request.QualificationsRequest!.Score.HasValue)
            {
                predicate = predicate
                    .And(x => x.Score == request.QualificationsRequest.Score);
            }

            if (!string.IsNullOrEmpty(request.QualificationsRequest!.Comment))
            {
                predicate = predicate
                    .And(x => x.Comment!.ToLower()
                    .Contains(request.QualificationsRequest.Comment.ToLower()));
            }

            if (request.QualificationsRequest!.CourseId.HasValue)
            {
                predicate = predicate
                    .And(x => x.CourseId == request.QualificationsRequest.CourseId);
            }

            if (!string.IsNullOrEmpty(request.QualificationsRequest!.OrderBy))
            {
                Expression<Func<Qualification, object>>? orderBySelector =
                request.QualificationsRequest.OrderBy!.ToLower() switch
                {
                    "score" => qualification => qualification.Score!,
                    "comment" => qualification => qualification.Comment!,
                    _ => qualification => qualification.CreatedAt
                };

                bool orderBy = request.QualificationsRequest.OrderAsc.HasValue
                            ? request.QualificationsRequest.OrderAsc.Value
                            : true;

                queryable = orderBy
                            ? queryable.OrderBy(orderBySelector)
                            : queryable.OrderByDescending(orderBySelector);
            }

            queryable = queryable.Where(predicate);

            var qualificationsQuery = queryable
            .ProjectTo<QualificationResponse>(_mapper.ConfigurationProvider)
            .AsQueryable();

            var pagination = await PagedList<QualificationResponse>.CreateAsync(
                qualificationsQuery,
                request.QualificationsRequest.PageNumber,
                request.QualificationsRequest.PageSize
            );

            return Result<PagedList<QualificationResponse>>.Success(pagination);
        }
    }
}

public record QualificationResponse(
    Guid Id,
    string? Student,
    int? Score,
    string? CourseTitle,
    string? Comment,
    Guid? CourseId,
    DateTime CreatedAt,
    DateTime UpdatedAt
);