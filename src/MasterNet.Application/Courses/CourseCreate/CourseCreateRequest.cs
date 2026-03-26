using Microsoft.AspNetCore.Http;
namespace MasterNet.Application.Courses.CourseCreate;

public class CourseCreateRequest
{
    public string? Title { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public IFormFile? Photo { get; set; }
    public Guid? InstructorId { get; set; }
    public Guid? PriceId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}