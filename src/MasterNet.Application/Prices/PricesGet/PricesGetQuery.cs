namespace MasterNet.Application.Prices.PricesGet;

public record PriceResponse(
    Guid? Id,
    string? Name,
    decimal? CurrentPrice,
    decimal? PromotionalPrice,
    DateTime CreatedAt,
    DateTime UpdatedAt
);