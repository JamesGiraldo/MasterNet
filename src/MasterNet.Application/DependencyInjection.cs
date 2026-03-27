using FluentValidation;
using FluentValidation.AspNetCore;
using MasterNet.Application.Courses.CourseCreate;
using Microsoft.Extensions.DependencyInjection;

namespace MasterNet.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(config => {
            config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });

        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<CourseCreateCommand>();

        return services;
    }
}