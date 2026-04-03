using MasterNet.Application.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static MasterNet.Application.Qualifications.QualificationsGet.QualificationsGetQuery;
using MasterNet.Application.Qualifications.QualificationsGet;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace MasterNet.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/qualifications")]
public class QuialificationController : ControllerBase
{
    private readonly ISender _sender;

    public QuialificationController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<PagedList<QualificationResponse>>> QuialificationsGetAll(
        [FromQuery] QualificationsGetRequest request,
        CancellationToken cancellationToken
    )
    {
        var query = new QualificationsGetQueryRequest { QualificationsRequest = request };
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
    }
}