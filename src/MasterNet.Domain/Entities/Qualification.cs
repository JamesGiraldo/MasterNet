using MasterNet.Domain.Common;

namespace MasterNet.Domain.Entities;

public class Qualification : BaseEntity
{
    public string? Student { get; set; }
    public int? Score { get; set; }
    public string? Comment { get; set; }
    public Guid? CourseId { get; set; }
    public Course? Course { get; set; }
}