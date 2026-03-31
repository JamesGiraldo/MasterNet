using MasterNet.Application.Core;

namespace MasterNet.Application.Prices.PricesGet;

public class PricesGetRequest : PagingParams
{
    public string? Name { get; set; }
    public decimal? CurrentPrice { get; set; }
    public decimal? PromotionalPrice { get; set; }
}