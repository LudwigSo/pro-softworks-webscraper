using Domain.Model;
using Domain.Ports.Queries;
using Microsoft.EntityFrameworkCore;

namespace Driven.Persistence.Postgres.Queries
{
    public class ProjectQueries(Context context) : IProjectQueriesPort
    {
        private readonly Context _context = context ?? throw new ArgumentNullException(nameof(context));

        public Task<Project[]> GetActiveBySource(ProjectSource source)
            => _context.Projects.Include(p => p.Tags).Where(x => x.Source == source && x.RemovedAt == null).ToArrayAsync();

        public Task<Project?> GetLastScrapedBySource(ProjectSource source)
            => _context.Projects.Include(p => p.Tags).Where(x => x.Source == source && x.RemovedAt == null).OrderByDescending(x => x.PostedAt).FirstOrDefaultAsync();

        public Task<Project[]> GetActiveWithAnyTag()
            => _context.Projects.Include(p => p.Tags).Where(x => x.RemovedAt == null && x.Tags.Count != 0).ToArrayAsync();

        public Task<Project[]> GetAll(int page, int skipPerPage, int take)
            => _context.Projects.Include(p => p.Tags).OrderBy(p => p.Id).Skip(page * skipPerPage).Take(take).ToArrayAsync();

        public Task<int> GetProjectCount()
            => _context.Projects.CountAsync();
    }
}
