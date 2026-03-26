using MasterNet.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MasterNet.Persistence.Identity;

namespace MasterNet.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbContext<MasterNetDbContext>(options =>
        {
            options.LogTo(
                Console.WriteLine,
                new[] {
                    DbLoggerCategory.Database.Command.Name,
                },
                LogLevel.Information
            ).EnableSensitiveDataLogging();

            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddScoped<IApplicationDbContext>(sp =>
            sp.GetRequiredService<MasterNetDbContext>()
        );
        services.AddScoped<IIdentityRoleInitializer, IdentityRoleInitializer>();

        return services;
    }
}