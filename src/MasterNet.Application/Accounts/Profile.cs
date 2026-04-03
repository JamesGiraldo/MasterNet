namespace MasterNet.Application.Accounts;

public class Profile
{
    public Guid Id { get; set; }
    public string? FullName { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public string? Token { get; set; } = string.Empty;
    public string? UserName { get; set; } = string.Empty;
}