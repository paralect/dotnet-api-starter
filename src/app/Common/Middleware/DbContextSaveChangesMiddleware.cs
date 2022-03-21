using System.Threading.Tasks;
using Common.DalSql;
using Microsoft.AspNetCore.Http;

namespace Common.Middleware;

public class DbContextSaveChangesMiddleware
{
    private readonly RequestDelegate _next;

    public DbContextSaveChangesMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, ShipDbContext dbContext)
    {
        await _next(context);
        await dbContext.SaveChangesAsync();
    }
}
