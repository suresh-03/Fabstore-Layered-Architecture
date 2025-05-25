using Fabstore.DataAccess.Configs;
using Fabstore.DataAccess.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
    public AppDbContext CreateDbContext(string[] args)
        {

        var connectionString = DbConfig.CONNECTION_STRING;

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new AppDbContext(optionsBuilder.Options);
        }
    }
