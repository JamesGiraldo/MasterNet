namespace MasterNet.Application.Instructors.InstructorsGet;

public record InstructorResponse(
    Guid? Id,
    string? Name,
    string? LastName,
    string? Degree,
    DateTime CreatedAt,
    DateTime UpdatedAt
);