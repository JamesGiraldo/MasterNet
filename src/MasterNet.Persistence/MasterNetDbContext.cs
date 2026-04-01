using MasterNet.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using MasterNet.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MasterNet.Persistence.Models;

namespace MasterNet.Persistence;

public class MasterNetDbContext : IdentityDbContext<User>, IApplicationDbContext
{

    public DbSet<Course> Courses { get; set; } = null!;
    public DbSet<Instructor>? Instructors { get; set; }
    public DbSet<Photo>? Photos { get; set; }
    public DbSet<Price>? Prices { get; set; }
    public DbSet<Qualification>? Qualifications { get; set; }

    public MasterNetDbContext() { }
    public MasterNetDbContext(DbContextOptions<MasterNetDbContext> options) : base(options) { }

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