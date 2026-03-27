using MasterNet.Application.Courses.CourseCreate;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static MasterNet.Application.Courses.CourseCreate.CourseCreateCommand;
using static MasterNet.Application.Courses.CourseReportExcel.CourseReportExcelQuery;

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
    public async Task<ActionResult<Guid>> CursoCreate(
        [FromForm] CourseCreateRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new CourseCreateCommandRequest(request);
        var resultado = await _sender.Send(command, cancellationToken);
        return Ok(resultado);
    }
}
