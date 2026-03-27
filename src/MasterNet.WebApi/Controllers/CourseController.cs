using MasterNet.Application.Core;
using MasterNet.Application.Courses.CourseCreate;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static MasterNet.Application.Courses.CourseCreate.CourseCreateCommand;
using static MasterNet.Application.Courses.CourseReportExcel.CourseReportExcelQuery;
using MasterNet.Application.Courses.CourseGet;

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

    [HttpGet("{id}")]
    public async Task<ActionResult> CourseGet(Guid id, CancellationToken cancellationToken)
    {
        var query = new CourseGetQuery.CourseGetQueryRequest { Id = id };
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
    }

    [HttpGet("report-excel")]
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
    public async Task<ActionResult<Result<Guid>>> CursoCreate(
        [FromForm] CourseCreateRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new CourseCreateCommandRequest(request);
        return await _sender.Send(command, cancellationToken);
    }
}
