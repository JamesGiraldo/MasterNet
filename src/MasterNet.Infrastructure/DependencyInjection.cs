using MasterNet.Application.Interfaces;
using MasterNet.Infrastructure.Reports;
using Microsoft.Extensions.DependencyInjection;

namespace MasterNet.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped(typeof(IReportService<>), typeof(ReportService<>));
        return services;
    }
}
