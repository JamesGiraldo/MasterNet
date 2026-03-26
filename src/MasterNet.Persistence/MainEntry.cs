// using MasterNet.Persistence;
// using MasterNet.Persistence.Models;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Logging;

// var services = new ServiceCollection();

// services.AddLogging(l =>
// {
//     l.ClearProviders();
// });

// services.AddDbContext<MasterNetDbContext>();

// services.AddIdentityCore<User>(options => {
//     options.Password.RequireDigit = true;
//     options.Password.RequiredLength = 6;
//     options.Password.RequireLowercase = true;
//     options.Password.RequireNonAlphanumeric = false;
//     options.Password.RequireUppercase = true;
//     options.User.RequireUniqueEmail = true;
// })
// .AddRoles<IdentityRole>()
// .AddEntityFrameworkStores<MasterNetDbContext>();

// var provider = services.BuildServiceProvider();

// try
// {
//     using var scope = provider.CreateAsyncScope();
//     var context = scope.ServiceProvider.GetRequiredService<MasterNetDbContext>();
//     await context.Database.MigrateAsync();
//     Console.WriteLine("Migraciones/Seedings aplicadas correctamente");

//     var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
//     var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
//     var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("SeedIdentity");

//     await SeedDatabase.SeedRolesAndUsersAsync(
//         userManager,
//         roleManager,
//         logger,
//         CancellationToken.None
//     );
// }
// catch (Exception ex)
// {
//     Console.WriteLine($"Error: {ex.Message}");
//     throw new Exception("Error al iniciar la aplicación", ex);
// }