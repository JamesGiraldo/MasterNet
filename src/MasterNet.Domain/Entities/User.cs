using Microsoft.AspNetCore.Identity;

namespace MasterNet.Domain.Entities;

public class User : IdentityUser
{
    public string? Name { get; set; }
    public string? LastName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
}
