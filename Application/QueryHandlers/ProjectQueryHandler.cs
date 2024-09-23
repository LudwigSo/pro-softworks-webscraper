using Application.QueryHandlers.Dtos;
using Domain;
using Driven.Persistence.Postgres;
using Microsoft.EntityFrameworkCore;

namespace Application.QueryHandlers;

public record ProjectsWithAnyTagQuery(DateTime Since);

public class ProjectQueryHandler(Context context)
{
    private readonly Context _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<ProjectDto[]> Handle(ProjectsWithAnyTagQuery query)
    {
        var projects = await _context
            .Projects
            .Include(p => p.Tags)
            .Where(x => x.FirstSeenAt >= query.Since && x.Tags.Count != 0)
            .ToArrayAsync();

        return projects.Select(ProjectDto.From).ToArray();
    }
}
