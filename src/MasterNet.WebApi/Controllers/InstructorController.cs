using MasterNet.Application.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static MasterNet.Application.Instructors.InstructorsGet.InstructorsGetQuery;
using MasterNet.Application.Instructors.InstructorsGet;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace MasterNet.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/instructors")]
public class InstructorsController : ControllerBase
{
    private readonly ISender _sender;

    public InstructorsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<PagedList<InstructorResponse>>> InstructorsGetAll(
        [FromQuery] InstructorsGetRequest request,
        CancellationToken cancellationToken
    )
    {
        var query = new InstructorsGetQueryRequest { InstructorsRequest = request };
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
    }
}