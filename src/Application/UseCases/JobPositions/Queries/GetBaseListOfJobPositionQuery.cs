using Application.Auth;
using Application.Persistence;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.JobPositions.Queries;

[Authorize(Roles = UserRoles.USER)]
public sealed record GetBaseListOfJobPositionQuery : IRequest<List<BaseJobPositionDto>>;
public sealed record BaseJobPositionDto(string Id, string PositionName)
{
    public static BaseJobPositionDto FromJobPosition(JobPosition jp)
        => new(jp.Id, jp.PositionBlueprint.Name);
};

public sealed class GetBaseListOfJobPositionQueryHandler(IAppDbContext context) : IRequestHandler<GetBaseListOfJobPositionQuery, List<BaseJobPositionDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<List<BaseJobPositionDto>> Handle(GetBaseListOfJobPositionQuery request, CancellationToken cancellationToken)
    {
        var jp = await _context.JobPositions
            .Include(jp => jp.PositionBlueprint)
            .Where(jp => jp.IsDeleted == false)
            .Where(jp => jp.IsVancant == true)
            .Select(jp => BaseJobPositionDto.FromJobPosition(jp))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var x = jp.DistinctBy(jp => jp.PositionName).ToList();

        return x;
    }
}