using System;
using Domain;

namespace Driven.Persistence.Postgres.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly Context _context;

        public ProjectRepository(Context context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddRange(List<Project> projects)
        {
            _context.Projects.AddRange(projects);
            await _context.SaveChangesAsync();
        }
    }
}