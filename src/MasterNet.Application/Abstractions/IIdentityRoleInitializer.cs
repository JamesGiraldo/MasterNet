namespace MasterNet.Application.Abstractions;

public interface IIdentityRoleInitializer
{
    Task EnsureInitialRolesAsync(CancellationToken cancellationToken = default);
}
