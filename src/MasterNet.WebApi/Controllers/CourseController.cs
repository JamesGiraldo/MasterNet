using MasterNet.Application.Core;
using MasterNet.Application.Courses.CourseCreate;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static MasterNet.Application.Courses.CourseCreate.CourseCreateCommand;
using static MasterNet.Application.Courses.CourseReportExcel.CourseReportExcelQuery;
using MasterNet.Application.Courses.CourseGet;
using MasterNet.Application.Courses.CoursesGet;
using MasterNet.Application.Courses.CourseUpdate;
using MasterNet.Application.Courses.CourseDelete;
using System.Net;


namespace MasterNet.WebApi.Controllers;

[ApiController]
[Route("api/courses")]
public class CourseController : ControllerBase
{
    private readonly ISender _sender;

    public CourseController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<PagedList<CourseResponse>>> CourseGetAll(
        [FromQuery] GetCoursesRequest request,
        CancellationToken cancellationToken
    )
    {
        var query = new CoursesGetQuery.CoursesGetQueryRequest { CoursesRequest = request };
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<Result<CourseResponse>>> CourseGet(Guid id, CancellationToken cancellationToken)
    {
        var query = new CourseGetQuery.CourseGetQueryRequest { Id = id };
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result) : BadRequest(result.Error);
    }

    [HttpGet("report-excel")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CourseReportExcel(
        CancellationToken cancellationToken
    )
    {
        var query = new CourseReportExcelQueryRequest();
        var result = await _sender.Send(query, cancellationToken);

        byte[] excelBytes = result.ToArray();
        return File(excelBytes, "text/csv", "courses.csv");
    }

    [HttpPost("create")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<Result<Guid>>> CursoCreate(
        [FromForm] CourseCreateRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new CourseCreateCommandRequest(request);
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
    }

    [HttpPut("{id}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<Result<Guid>>> CourseUpdate(
        [FromBody] CourseUpdateRequest request,
        Guid id,
        CancellationToken cancellationToken
    )
    {
        var command = new CourseUpdateCommand.CourseUpdateCommandRequest(request, id);
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<Result<Unit>>> CourseDelete(Guid id, CancellationToken cancellationToken)
    {
        var command = new CurseDeteleCommand.CourseDeleteCommandRequest(CourseId: id);
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
    }
}
