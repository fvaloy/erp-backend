using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Persistence;

public interface IAppDbContext
{
    DbSet<Person> Persons { get; }
    DbSet<Employee> Employees { get; }
    DbSet<JobPosition> JobPositions { get; }
    DbSet<PositionBlueprint> PositionBlueprints { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}