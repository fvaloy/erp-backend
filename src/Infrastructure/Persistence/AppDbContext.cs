using Application.Auth;
using Application.Persistence;
using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser>, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {

    }

    public DbSet<Person> Persons => Set<Person>();

    public DbSet<Employee> Employees => Set<Employee>();

    public DbSet<JobPosition> JobPositions => Set<JobPosition>();

    public DbSet<PositionBlueprint> PositionBlueprints => Set<PositionBlueprint>();
}