using MasterNet.Application.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace MasterNet.Persistence.Identity;

public sealed class IdentityRoleInitializer : IIdentityRoleInitializer
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<IdentityRoleInitializer> _logger;

    public IdentityRoleInitializer(
        RoleManager<IdentityRole> roleManager,
        ILogger<IdentityRoleInitializer> logger
    )
    {
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task EnsureInitialRolesAsync(CancellationToken cancellationToken = default)
    {
        await SeedDatabase.SeedInitialRolesAsync(_roleManager, _logger).WaitAsync(cancellationToken);
    }
}
