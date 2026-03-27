using FluentValidation;
using FluentValidation.AspNetCore;
using MasterNet.Application.Core;
using MasterNet.Application.Courses.CourseCreate;
using MasterNet.Application.Courses.CourseGet;
using Microsoft.Extensions.DependencyInjection;

namespace MasterNet.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });

        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<CourseCreateCommand>();
        services.AddValidatorsFromAssemblyContaining<CourseGetQuery>();

        services.AddAutoMapper(_ => { }, typeof(MappingProfile).Assembly);

        return services;
    }
}
