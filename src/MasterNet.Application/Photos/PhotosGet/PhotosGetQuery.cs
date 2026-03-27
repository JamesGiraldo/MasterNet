namespace MasterNet.Application.Photos.PhotosGet;

public record PhotoResponse(
    Guid Id,
    string? Url,
    Guid? CourseId,
    DateTime CreatedAt,
    DateTime UpdatedAt
);