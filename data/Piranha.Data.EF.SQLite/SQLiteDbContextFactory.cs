using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Diagnostics.CodeAnalysis;

namespace Piranha.Data.EF.SQLite
{
    [ExcludeFromCodeCoverage]
    public class SQLiteDbContextFactory : IDesignTimeDbContextFactory<SQLiteDb>
    {
        public SQLiteDb CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SQLiteDb>();

            // Use a temporary connection string for design-time (migrations)
            optionsBuilder.UseSqlite("Data Source=piranha.db");

            // Suppress the pending model changes warning
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));

            return new SQLiteDb(optionsBuilder.Options);
        }
    }
}
