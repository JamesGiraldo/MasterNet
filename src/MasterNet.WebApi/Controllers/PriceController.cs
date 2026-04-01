using MasterNet.Application.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static MasterNet.Application.Prices.PricesGet.PricesGetQuery;
using MasterNet.Application.Prices.PricesGet;
using System.Net;

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
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<PagedList<PriceResponse>>> PricesGetAll(
        [FromQuery] PricesGetRequest request,
        CancellationToken cancellationToken
    )
    {
        var query = new PricesGetQueryRequest { PricesRequest = request };
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
    }
}