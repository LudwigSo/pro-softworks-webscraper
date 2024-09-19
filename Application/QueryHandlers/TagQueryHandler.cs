using Application.QueryHandlers.Dtos;
using Domain;
using Driven.Persistence.Postgres;
using Microsoft.EntityFrameworkCore;

namespace Application.QueryHandlers;

public record AllTagsQuery();
public class TagQueryHandler(Context context)
{
    private readonly Context _context = context ?? throw new ArgumentNullException(nameof(context));

    public Task<TagDto[]> Handle(AllTagsQuery query)
        => _context.Tags.Include(t => t.Keywords).Select(t => TagDto.From(t)).ToArrayAsync();
        

    public Task<Tag?> GetTag(int id)
        => _context.Tags.SingleOrDefaultAsync(x => x.Id == id);
}
