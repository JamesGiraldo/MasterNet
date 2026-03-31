using MasterNet.Application.Core;

namespace MasterNet.Application.Instructors.InstructorsGet;

public class InstructorsGetRequest : PagingParams
{
    public string? Name { get; set; }
    public string? LastName { get; set; }
    public string? Degree { get; set; }
}