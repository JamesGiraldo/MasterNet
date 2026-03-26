using MasterNet.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Frozen;
using System.Globalization;

namespace MasterNet.Persistence;

public static class SeedDatabase
{
    /// <summary>
    /// Crea los roles <see cref="CustomRole.ADMIN"/> y <see cref="CustomRole.CLIENT"/> si no existen.
    /// Los métodos de seed de datos de demostración (JSON) permanecen disponibles para ejecución manual.
    /// </summary>
    public static async Task SeedInitialRolesAsync(
        RoleManager<IdentityRole> roleManager,
        ILogger? logger
    )
    {
        try
        {
            if (!await roleManager.RoleExistsAsync(CustomRole.ADMIN))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(CustomRole.ADMIN));
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(
                        string.Join("; ", result.Errors.Select(e => e.Description))
                    );
                }
            }

            if (!await roleManager.RoleExistsAsync(CustomRole.CLIENT))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(CustomRole.CLIENT));
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(
                        string.Join("; ", result.Errors.Select(e => e.Description))
                    );
                }
            }

            logger?.LogInformation("Roles iniciales ADMIN y CLIENT verificados");
        }
        catch (Exception ex)
        {
            logger?.LogWarning(ex, "Error al sembrar roles: {ErrorMessage}", ex.Message);
            throw new InvalidOperationException("Error al sembrar los roles iniciales", ex);
        }
    }

    public static async Task SeedPricesAsync(
        MasterNetDbContext dbContext,
        ILogger? logger,
        CancellationToken cancellationToken
    )
    {
        try
        {
            if (dbContext.Prices is null || dbContext.Prices.Any()) return;
            var jsonString = GetJsonFile("prices.json");
            var prices = JsonConvert.DeserializeObject<List<Price>>(jsonString);

            if (prices is null || !prices.Any()) return;

            dbContext.Prices.AddRange(prices!);
            await dbContext.SaveChangesAsync(cancellationToken);

            logger?.LogInformation("Precios sembrados correctamente");
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error al sembrar los precios: {ErrorMessage}", ex.Message);
            throw new Exception("Error al sembrar los precios", ex);
        }
    }

    public static async Task SeedInstructorsAsync(
        MasterNetDbContext dbContext,
        ILogger? logger,
        CancellationToken cancellationToken
    )
    {
        try
        {
            if (dbContext.Instructors is null || dbContext.Instructors.Any()) return;
            var jsonString = GetJsonFile("instructors.json");
            var instructors = JsonConvert.DeserializeObject<List<Instructor>>(jsonString);

            if (instructors is null || !instructors.Any()) return;

            dbContext.Instructors.AddRange(instructors!);
            await dbContext.SaveChangesAsync(cancellationToken);

            logger?.LogInformation("Instructores sembrados correctamente");
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error al sembrar los instructores: {ErrorMessage}", ex.Message);
            throw new Exception("Error al sembrar los instructores", ex);
        }
    }

    public static async Task SeedCoursesAsync(
        MasterNetDbContext dbContext,
        ILogger? logger,
        CancellationToken cancellationToken
    )
    {
        try
        {
            if (dbContext.Courses is null || dbContext.Courses.Any()) return;
            var jsonString = GetJsonFile("courses.json");
            // var courses = JsonConvert.DeserializeObject<List<Course>>(jsonString);

            var instructors = dbContext.Instructors!.ToFrozenDictionary(i => i.Id, i => i);
            var prices = dbContext.Prices!.ToFrozenDictionary(p => p.Id, p => p);

            var arrayCourses = JArray.Parse(jsonString);

            var coursesDb = new List<Course>();

            foreach (var objectCourse in arrayCourses)
            {
                var idString = objectCourse["Id"]?.ToString();

                if (!Guid.TryParse(idString, out var id))
                {
                    id = Guid.NewGuid();
                }

                var title = objectCourse["Title"]?.ToString();
                var description = objectCourse["Description"]?.ToString();

                DateTime? createdAt = null;
                var createdAtString = objectCourse["CreatedAt"]?.ToString();

                if (
                    !string.IsNullOrEmpty(createdAtString) &&
                    DateTime.TryParse(
                        createdAtString,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.RoundtripKind,
                        out var createdAtDate
                    )
                )
                {
                    createdAt =
                        createdAtDate.Kind == DateTimeKind.Unspecified
                            ? DateTime.SpecifyKind(createdAtDate, DateTimeKind.Utc)
                            : createdAtDate.ToUniversalTime();
                }

                var course = new Course
                {
                    Id = id,
                    Title = title,
                    Description = description,
                    CreatedAt = createdAt ?? DateTime.UtcNow,
                    Qualifications = new List<Qualification>(),
                    Prices = new List<Price>(),
                    Instructors = new List<Instructor>(),
                    CoursePrices = new List<CoursePrice>(),
                    CourseInstructors = new List<CourseInstructor>(),
                    Photos = new List<Photo>(),
                };

                if (objectCourse["Prices"] is JArray pricesArray)
                {
                    foreach (var priceId in pricesArray)
                    {
                        var priceIdGuid = new Guid(priceId.ToString());

                        if (prices.TryGetValue(priceIdGuid, out var price))
                        {
                            course.Prices.Add(price);
                        }

                    }
                }

                if (objectCourse["Instructors"] is JArray instructorsArray)
                {
                    foreach (var instructorId in instructorsArray)
                    {
                        var instructorIdGuid = new Guid(instructorId.ToString());
                        if (instructors.TryGetValue(instructorIdGuid, out var instructor))
                        {
                            course.Instructors.Add(instructor);
                        }
                    }
                }

                coursesDb.Add(course);
            }

            dbContext.Courses.AddRange(coursesDb);
            await dbContext.SaveChangesAsync(cancellationToken);

            logger?.LogInformation("Cursos sembrados correctamente");
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error al sembrar los cursos: {ErrorMessage}", ex.Message);
            throw new Exception("Error al sembrar los cursos", ex);
        }
    }

    public static async Task SeedQualificationsAsync(
        MasterNetDbContext dbContext,
        ILogger? logger,
        CancellationToken cancellationToken
    )
    {
        try
        {
            if (dbContext.Qualifications is null || dbContext.Qualifications.Any()) return;
            var jsonString = GetJsonFile("qualifications.json");
            var qualifications = JsonConvert.DeserializeObject<List<Qualification>>(jsonString);

            if (qualifications is null || !qualifications.Any()) return;

            foreach (var qu in qualifications)
            {
                qu.Course = null;
            }

            dbContext.Qualifications.AddRange(qualifications!);
            await dbContext.SaveChangesAsync(cancellationToken);

            logger?.LogInformation("Calificaciones sembradas correctamente");
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error al sembrar las calificaciones: {ErrorMessage}", ex.Message);
            throw new Exception("Error al sembrar las calificaciones", ex);
        }
    }

    private static string GetJsonFile(string fileName)
    {
        // Leer el archivo de forma 1
        var leerForma1 = Path.Combine(
            Directory.GetCurrentDirectory(),
            "src",
            "MasterNet.Persistence",
            "SeedData",
            fileName
        );

        // Leer el archivo de forma 2
        var leerForma2 = Path.Combine(
            Directory.GetCurrentDirectory(),
            "SeedData",
            fileName
        );

        // Leer el archivo de forma 3
        var leerForma3 = Path.Combine(
            AppContext.BaseDirectory,
            "SeedData",
            fileName
        );

        if (File.Exists(leerForma1)) return File.ReadAllText(leerForma1);
        if (File.Exists(leerForma2)) return File.ReadAllText(leerForma2);
        if (File.Exists(leerForma3)) return File.ReadAllText(leerForma3);

        throw new FileNotFoundException($"No se encontró el archivo {fileName}");
    }
}