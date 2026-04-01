using MasterNet.Application.Courses.CourseCreate;
namespace MasterNet.Application.Courses.CourseUpdate;

public class CourseUpdateRequest : CourseCreateRequest
{
    public Guid Id { get; set; }
}