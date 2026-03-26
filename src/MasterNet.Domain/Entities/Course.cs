using MasterNet.Domain.Common;

namespace MasterNet.Domain.Entities;

public class Course : BaseEntity
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public ICollection<Qualification>? Qualifications { get; set; }
    public ICollection<Photo>? Photos { get; set; }

    public ICollection<Price>? Prices { get; set; }
    public ICollection<CoursePrice>? CoursePrices { get; set; }
    public ICollection<Instructor>? Instructors { get; set; }
    public ICollection<CourseInstructor>? CourseInstructors { get; set; }
}