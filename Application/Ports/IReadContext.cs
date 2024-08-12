using Domain.Model;

namespace Application.Ports;

public interface IReadContext
{
    IQueryable<Project> Projects { get; }
    Task<List<T>> ToListAsync<T>(IQueryable<T> query) where T : class;
}