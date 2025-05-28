using Fabstore.DataAccess.Configs;
using Fabstore.DataAccess.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

// Factory class for creating AppDbContext instances at design time (e.g., for migrations)
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
    // Creates a new AppDbContext instance using the configured connection string
    public AppDbContext CreateDbContext(string[] args)
        {
        // Retrieve the connection string from configuration
        var connectionString = DbConfig.CONNECTION_STRING;

        // Build the DbContext options using SQL Server provider
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        // Return a new AppDbContext with the configured options
        return new AppDbContext(optionsBuilder.Options);
        }
    }
