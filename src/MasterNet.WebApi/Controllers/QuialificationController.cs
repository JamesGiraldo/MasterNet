using MasterNet.Application.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static MasterNet.Application.Qualifications.QualificationsGet.QualificationsGetQuery;
using MasterNet.Application.Qualifications.QualificationsGet;

namespace MasterNet.WebApi.Controllers;

[ApiController]
[Route("api/qualifications")]
public class QuialificationController : ControllerBase
{
    private readonly ISender _sender;

    public QuialificationController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult> QuialificationsGetAll(
        [FromQuery] QualificationsGetRequest request,
        CancellationToken cancellationToken
    )
    {
        var query = new QualificationsGetQueryRequest { QualificationsRequest = request };
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
    }
}