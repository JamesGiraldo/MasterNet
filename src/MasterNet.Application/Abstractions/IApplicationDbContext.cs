using MasterNet.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MasterNet.Application.Abstractions;

public interface IApplicationDbContext
{
    DbSet<Course> Courses { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
