using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace ProductService.Infrastructure.DbContexts;

public class DapperDbContext
{
    public DapperDbContext(IConfiguration configuration)
    {
        string? stringConnection = configuration.GetConnectionString("PostgresConnection");

        Connection = new NpgsqlConnection(stringConnection);
    }
    
    public IDbConnection Connection { get; }
}