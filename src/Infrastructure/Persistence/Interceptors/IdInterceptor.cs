using Application.Auth;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Persistence.Interceptors;

public class IdInterceptor(IUser user) : SaveChangesInterceptor
{
    private readonly IUser _user = user;

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        AddId(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    static void AddId(DbContext? context)
    {
        if (context is null) return;
        foreach (var entry in context.ChangeTracker.Entries<IEntity>())
        {
            if (entry.State is EntityState.Added)
            {
                entry.Entity.Id = Guid.NewGuid().ToString();
            }
        }
    }
}