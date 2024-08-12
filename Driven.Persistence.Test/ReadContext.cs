using Application.Ports;
using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Driven.Persistence.Test;

public class TestReadContext : IReadContext
{
    public TestReadContext() { }

    public IQueryable<Project> Projects => new List<Project>().AsQueryable();
    public async Task<List<T>> ToListAsync<T>(IQueryable<T> query) where T : class
    {
        return await query.ToListAsync();
    }
}