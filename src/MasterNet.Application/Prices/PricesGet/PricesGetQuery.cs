using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MasterNet.Application.Abstractions;
using MasterNet.Application.Core;
using MasterNet.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MasterNet.Application.Prices.PricesGet;

public class PricesGetQuery : IRequest<Result<PagedList<PriceResponse>>>
{
    public record PricesGetQueryRequest : IRequest<Result<PagedList<PriceResponse>>>
    {
        public PricesGetRequest? PricesRequest { get; set; }
    }

    internal class PricesGetQueryHandler
    : IRequestHandler<PricesGetQueryRequest, Result<PagedList<PriceResponse>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public PricesGetQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<PagedList<PriceResponse>>> Handle(
            PricesGetQueryRequest request,
            CancellationToken cancellationToken
        )
        {
            IQueryable<Price> queryable = _context.Prices!
                                            .Include(x => x.Courses)
                                            .Include(x => x.CoursePrices);

            var predicate = ExpressionBuilder.New<Price>();
            if (!string.IsNullOrEmpty(request.PricesRequest!.Name))
            {
                predicate = predicate.And(x => x.Name!.ToLower().Contains(request.PricesRequest.Name.ToLower()));
            }

            if (request.PricesRequest!.CurrentPrice.HasValue)
            {
                predicate = predicate.And(x => x.CurrentPrice == request.PricesRequest.CurrentPrice);
            }

            if (request.PricesRequest!.PromotionalPrice.HasValue)
            {
                predicate = predicate.And(x => x.PromotionalPrice == request.PricesRequest.PromotionalPrice);
            }

            if (!string.IsNullOrEmpty(request.PricesRequest!.OrderBy))
            {
                Expression<Func<Price, object>>? orderBySelector = request.PricesRequest.OrderBy!.ToLower() switch
                {
                    "name" => price => price.Name!,
                    "currentprice" => price => price.CurrentPrice!,
                    "promotionalprice" => price => price.PromotionalPrice!,
                    _ => price => price.CreatedAt
                };

                bool orderBy = request.PricesRequest.OrderAsc.HasValue
                            ? request.PricesRequest.OrderAsc.Value
                            : true;

                queryable = orderBy
                            ? queryable.OrderBy(orderBySelector)
                            : queryable.OrderByDescending(orderBySelector);
            }

            queryable = queryable.Where(predicate);

            var pricesQuery = queryable
            .ProjectTo<PriceResponse>(_mapper.ConfigurationProvider)
            .AsQueryable();

            var pagination = await PagedList<PriceResponse>.CreateAsync(
                pricesQuery,
                request.PricesRequest.PageNumber,
                request.PricesRequest.PageSize
            );

            return Result<PagedList<PriceResponse>>.Success(pagination);
        }
    }

}

public record PriceResponse(
    Guid? Id,
    string? Name,
    decimal? CurrentPrice,
    decimal? PromotionalPrice,
    DateTime CreatedAt,
    DateTime UpdatedAt
);