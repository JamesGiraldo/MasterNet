using MasterNet.Application.Interfaces;
using MasterNet.Domain.Entities;
using MasterNet.Infrastructure.Security;
using MasterNet.Persistence;
using Microsoft.AspNetCore.Identity;

namespace MasterNet.WebApi.Extensions;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {

        services
            .AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<MasterNetDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserAccessor, UserAccessor>();

        return services;
    }
}