using MasterNet.Domain.Common;

namespace MasterNet.Domain.Entities;

public class Instructor : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Degree { get; set; } = string.Empty;

    public ICollection<Course>? Courses { get; set; }
    public ICollection<CourseInstructor>? CourseInstructors { get; set; }
}