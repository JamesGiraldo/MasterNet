using MasterNet.Application.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static MasterNet.Application.Prices.PricesGet.PricesGetQuery;
using MasterNet.Application.Prices.PricesGet;

namespace MasterNet.WebApi.Controllers;

[ApiController]
[Route("api/prices")]
public class PriceController : ControllerBase
{
    private readonly ISender _sender;

    public PriceController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult> PricesGetAll(
        [FromQuery] PricesGetRequest request,
        CancellationToken cancellationToken
    )
    {
        var query = new PricesGetQueryRequest { PricesRequest = request };
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
    }
}