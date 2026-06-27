using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Respawn;

namespace Wakiliy.API.IntegrationTests.Infrastructure;

public class DatabaseResetter
{
    private static Respawner _respawner = default!;

    public static async Task InitializeAsync(string connectionString)
    {
        if (_respawner != null) return;

        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        
        _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.SqlServer,
            SchemasToInclude = new[] { "dbo" },
            // EF Core migrations history table and identity roles should not be cleared
            TablesToIgnore = new Respawn.Graph.Table[] { "__EFMigrationsHistory", "AspNetRoles" }
        });
    }

    public static async Task ResetAsync(string connectionString)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        await _respawner.ResetAsync(connection);
    }
}
