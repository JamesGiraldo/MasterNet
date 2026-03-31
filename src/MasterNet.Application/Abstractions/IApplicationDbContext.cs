using MasterNet.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MasterNet.Application.Abstractions;

public interface IApplicationDbContext
{
    DbSet<Course> Courses { get; }
    DbSet<Instructor> Instructors { get; }
    DbSet<Photo> Photos { get; }
    DbSet<Price> Prices { get; }
    DbSet<Qualification> Qualifications { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
