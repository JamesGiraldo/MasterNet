using Microsoft.EntityFrameworkCore;
using MasterNet.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MasterNet.Persistence.Models;

namespace MasterNet.Persistence;

public class MasterNetDbContext : IdentityDbContext<User>
{

    public DbSet<Course>? Courses { get; set; }
    public DbSet<Instructor>? Instructors { get; set; }
    public DbSet<Photo>? Photos { get; set; }
    public DbSet<Price>? Prices { get; set; }
    public DbSet<Qualification>? Qualifications { get; set; }

    public MasterNetDbContext() { }
    public MasterNetDbContext(DbContextOptions<MasterNetDbContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=masternet1;Username=postgres;Password=postgres")
            .EnableDetailedErrors()
            .LogTo(Console.WriteLine, LogLevel.Information)
            .EnableSensitiveDataLogging()
            .UseAsyncSeeding(async (context, status, cancellationToken) =>
                {
                    var masterNetDbContext = (MasterNetDbContext)context;
                    var logger = context.GetService<ILogger<MasterNetDbContext>>();
                    try
                    {
                        await SeedDatabase.SeedPricesAsync(
                            masterNetDbContext,
                            logger,
                            cancellationToken
                        );
                        await SeedDatabase.SeedInstructorsAsync(
                            masterNetDbContext,
                            logger,
                            cancellationToken
                        );
                        await SeedDatabase.SeedCoursesAsync(
                            masterNetDbContext,
                            logger,
                            cancellationToken
                        );
                        await SeedDatabase.SeedQualificationsAsync(
                            masterNetDbContext,
                            logger,
                            cancellationToken
                        );
                    }
                    catch (Exception ex)
                    {
                        logger?.LogError(ex, "Error en el seeding: {ErrorMessage}", ex.Message);
                        throw new Exception("Error en el seeding", ex);
                    }
                }
            );
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Course>().ToTable("courses");
        modelBuilder.Entity<CourseInstructor>().ToTable("course_instructors");
        modelBuilder.Entity<CoursePrice>().ToTable("course_prices");
        modelBuilder.Entity<Instructor>().ToTable("instructors");
        modelBuilder.Entity<Photo>().ToTable("photos");
        modelBuilder.Entity<Price>().ToTable("prices");
        modelBuilder.Entity<Qualification>().ToTable("qualifications");

        modelBuilder.Entity<Price>()
            .Property(p => p.CurrentPrice)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Price>()
            .Property(p => p.PromotionalPrice)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Price>()
            .Property(p => p.Name)
            .HasColumnType("VARCHAR")
            .HasMaxLength(250);

        // Relationships
        modelBuilder.Entity<Course>()
            .HasMany(c => c.Photos)
            .WithOne(p => p.Course)
            .HasForeignKey(p => p.CourseId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Course>()
            .HasMany(c => c.Qualifications)
            .WithOne(q => q.Course)
            .HasForeignKey(q => q.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Course>()
            .HasMany(c => c.Prices)
            .WithMany(p => p.Courses)
            .UsingEntity<CoursePrice>(
                cp => cp
                    .HasOne(cp => cp.Price)
                    .WithMany(p => p.CoursePrices)
                    .HasForeignKey(cp => cp.PriceId),

                cp => cp
                    .HasOne(cp => cp.Course)
                    .WithMany(c => c.CoursePrices)
                    .HasForeignKey(cp => cp.CourseId),

                j =>
                {
                    j.HasKey(t => new { t.CourseId, t.PriceId });
                }
            );

        modelBuilder.Entity<Course>()
            .HasMany(c => c.Instructors)
            .WithMany(i => i.Courses)
            .UsingEntity<CourseInstructor>(
                ci => ci
                    .HasOne(ci => ci.Instructor)
                    .WithMany(i => i.CourseInstructors)
                    .HasForeignKey(ci => ci.InstructorId),

                ci => ci
                    .HasOne(ci => ci.Course)
                    .WithMany(c => c.CourseInstructors)
                    .HasForeignKey(ci => ci.CourseId),

                j =>
                {
                    j.HasKey(t => new { t.CourseId, t.InstructorId });
                }
            );
    }

}