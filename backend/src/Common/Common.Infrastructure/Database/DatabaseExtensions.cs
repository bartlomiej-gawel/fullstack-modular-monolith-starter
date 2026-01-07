using Common.Infrastructure.Database.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Common.Infrastructure.Database;

public static class DatabaseExtensions
{
    extension(IServiceCollection services)
    {
        internal void AddDatabase(IConfiguration configuration)
        {
            services.AddOptions<DatabaseOptions>()
                .Bind(configuration.GetSection(DatabaseOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddScoped<DomainEventInterceptor>();

            services.AddHostedService<DatabaseInitializer>();
        }

        public void AddDatabaseContext<T>(string schemaName)
            where T : ApplicationDbContext
        {
            services.AddDbContext<T>((sp, options) =>
            {
                var databaseOptions = sp.GetRequiredService<IOptions<DatabaseOptions>>().Value;

                options.UseNpgsql(
                    databaseOptions.ConnectionString,
                    npgsqlOptions =>
                    {
                        npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, schemaName);
                    });

                if (databaseOptions.EnableDetailedErrors)
                    options.EnableDetailedErrors();

                if (databaseOptions.EnableSensitiveDataLogging)
                    options.EnableSensitiveDataLogging();

                options.AddInterceptors(sp.GetRequiredService<DomainEventInterceptor>());
            });
        }
    }
}
