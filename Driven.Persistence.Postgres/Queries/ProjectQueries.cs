using Domain.Model;
using Domain.Ports;
using Microsoft.EntityFrameworkCore;

namespace Driven.Persistence.Postgres.Queries
{
    public class ProjectQueries(Context context) : IProjectQueriesPort
    {
        private readonly Context _context = context ?? throw new ArgumentNullException(nameof(context));

        public Task<Project[]> GetActiveBySource(ProjectSource source)
            => _context.Projects.Where(x => x.Source == source && x.RemovedAt == null).ToArrayAsync();

        public Task<Project[]> GetActive()
            => _context.Projects.Where(x => x.RemovedAt == null).ToArrayAsync();

        public Task<Project[]> GetAll(int page, int skipPerPage, int take)
            => _context.Projects.OrderBy(p => p.Id).Skip(page * skipPerPage).Take(take).ToArrayAsync();

        public Task<int> GetProjectCount()
            => _context.Projects.CountAsync();

        public Task<Tag[]> GetAllTags()
            => _context.Tags.ToArrayAsync();
    }
}
