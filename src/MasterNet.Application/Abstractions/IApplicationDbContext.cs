using MasterNet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace MasterNet.Application.Abstractions;

public interface IApplicationDbContext
{
    DatabaseFacade Database { get; }
    DbSet<Course> Courses { get; }
    DbSet<Instructor> Instructors { get; }
    DbSet<Photo> Photos { get; }
    DbSet<Price> Prices { get; }
    DbSet<Qualification> Qualifications { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
