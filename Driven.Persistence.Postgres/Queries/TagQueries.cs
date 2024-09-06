using Domain.Model;
using Domain.Ports.Queries;
using Microsoft.EntityFrameworkCore;

namespace Driven.Persistence.Postgres.Queries
{
    public class TagQueries(Context context) : ITagQueriesPort
    {
        private readonly Context _context = context ?? throw new ArgumentNullException(nameof(context));

        public Task<Tag[]> GetAllTags()
            => _context.Tags.ToArrayAsync();

        public Task<Tag?> GetTag(int id)
            => _context.Tags.SingleOrDefaultAsync(x => x.Id == id);
    }
}
