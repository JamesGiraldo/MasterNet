namespace MasterNet.WebApi.Extensions;

using MasterNet.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;

public static class DataSeed
{
    public static async Task SeedDataAuthenticationAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateAsyncScope();
        var service = scope.ServiceProvider;
        var loggerFactory = service.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("DataSeed");

        try
        {
            var initializer = service.GetRequiredService<IIdentityRoleInitializer>();
            await initializer.EnsureInitialRolesAsync(CancellationToken.None);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al sembrar roles iniciales");
            throw;
        }
    }
}