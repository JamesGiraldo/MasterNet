namespace MasterNet.Application.Qualifications.QualificationsGet;

public record QualificationResponse(
    Guid Id,
    string? Student,
    int? Score,
    string? CourseTitle,
    string? Comment,
    Guid? CourseId,
    DateTime CreatedAt,
    DateTime UpdatedAt
);