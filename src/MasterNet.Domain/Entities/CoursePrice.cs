using MasterNet.Domain.Common;

namespace MasterNet.Domain.Entities;

public class CoursePrice
{
    public Guid? CourseId { get; set; }
    public Course? Course { get; set; }

    public Guid? PriceId { get; set; }
    public Price? Price { get; set; }
}