using MasterNet.Domain.Common;

namespace MasterNet.Domain.Entities;

public class Photo : BaseEntity
{
    public string? Url { get; set; }
    public Guid? CourseId { get; set; }
    public Course? Course { get; set; }
}