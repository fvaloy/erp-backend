using Application.Auth;
using Application.Persistence;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Persons.Queries;

[Authorize(Roles = UserRoles.USER)]
public sealed record GetBaseListOfPositionBlueprintQuery : IRequest<List<BasePositionBlueprintDto>>;
public sealed record BasePositionBlueprintDto(string Id, string Name)
{
    public static BasePositionBlueprintDto FromPositionBlueprint(PositionBlueprint p)
        => new (p.Id, p.Name);
};

public sealed class GetBaseListOfPositionBlueprintQueryHandler(IAppDbContext context) : IRequestHandler<GetBaseListOfPositionBlueprintQuery, List<BasePositionBlueprintDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<List<BasePositionBlueprintDto>> Handle(GetBaseListOfPositionBlueprintQuery request, CancellationToken cancellationToken)
    {
        var pb = await _context.PositionBlueprints
            .Where(p => p.IsDeleted == false)
            .Select(p => BasePositionBlueprintDto.FromPositionBlueprint(p))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        
        return pb;
    }
}