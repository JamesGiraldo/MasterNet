using MasterNet.Application.Abstractions;
using MasterNet.Application.Interfaces;
using MasterNet.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MasterNet.Application.Courses.CourseReportExcel;

public class CourseReportExcelQuery
{
    public record CourseReportExcelQueryRequest : IRequest<MemoryStream>;

    internal class CourseReportExcelQueryHandler : IRequestHandler<CourseReportExcelQueryRequest, MemoryStream>
    {

        private readonly IApplicationDbContext _context;
        private readonly IReportService<Course> _reportService;

        public CourseReportExcelQueryHandler(IApplicationDbContext context, IReportService<Course> reportService)
        {
            _context = context;
            _reportService = reportService;
        }

        public async Task<MemoryStream> Handle(
            CourseReportExcelQueryRequest request,
            CancellationToken cancellationToken
        )
        {
            var courses = await _context.Courses.Take(10).Skip(0).AsNoTracking().ToListAsync();

            return await _reportService.GetCsvReport(courses);
        }
    }
}