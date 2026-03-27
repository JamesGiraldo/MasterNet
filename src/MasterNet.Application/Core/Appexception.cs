namespace MasterNet.Application.Core;

public class AppException : Exception
{
    public AppException(int statusCode, string message, string? details = null)
        : base(message ?? string.Empty)
    {
        StatusCode = statusCode;
        Details = details;
    }

    public int StatusCode { get; set; }
    public string? Details { get; set; }
}