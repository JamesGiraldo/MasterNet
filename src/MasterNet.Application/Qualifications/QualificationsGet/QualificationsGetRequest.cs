using MasterNet.Application.Core;

namespace MasterNet.Application.Qualifications.QualificationsGet;

public class QualificationsGetRequest : PagingParams
{
    public string? Student { get; set; }
    public Guid? CourseId { get; set; }
    public int? Score { get; set; }
    public string? Comment { get; set; }
}