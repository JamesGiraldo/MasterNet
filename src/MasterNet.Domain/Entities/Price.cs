using MasterNet.Domain.Common;

namespace MasterNet.Domain.Entities;

public class Price : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal CurrentPrice { get; set; }
    public decimal PromotionalPrice { get; set; }

    public ICollection<Course>? Courses { get; set; }
    public ICollection<CoursePrice>? CoursePrices { get; set; }
}