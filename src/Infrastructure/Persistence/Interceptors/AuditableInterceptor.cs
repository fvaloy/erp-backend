using Application.Auth;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Persistence.Interceptors;

public class AuditableInterceptor(IUser user) : SaveChangesInterceptor
{
    private readonly IUser _user = user;

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public void UpdateEntities(DbContext? context)
    {
        if (context is null) return;
        foreach (var entry in context.ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State is EntityState.Added)
            {
                entry.Entity.Id = Guid.NewGuid().ToString();
                entry.Entity.CreatedBy = _user.Id!;
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.Version = 1;
            }

            if (entry.State is EntityState.Modified)
            {
                entry.Entity.ModifiedBy = _user.Id!;
                entry.Entity.ModifiedAt = DateTime.UtcNow;
                entry.Entity.Version += 1;
            }
        }
    }
}