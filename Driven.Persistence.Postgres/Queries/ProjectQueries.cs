using Domain.Model;
using Domain.Ports;
using Microsoft.EntityFrameworkCore;

namespace Driven.Persistence.Postgres.Queries
{
    public class ProjectQueries(Context context) : IProjectQueriesPort
    {
        private readonly Context _context = context ?? throw new ArgumentNullException(nameof(context));

        public Task<List<Project>> GetActiveBySource(ProjectSource source)
        {
            return _context.Projects.Where(x => x.Source == source && x.RemovedAt == null).ToListAsync();
        }
    }
}
