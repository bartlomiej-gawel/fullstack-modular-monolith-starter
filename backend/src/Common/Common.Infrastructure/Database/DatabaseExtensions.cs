using Common.Infrastructure.Database.Interceptors;
using Common.Infrastructure.Domain;
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
        internal IServiceCollection AddDatabase(IConfiguration configuration)
        {
            services.AddOptions<DatabaseOptions>()
                .Bind(configuration.GetSection(DatabaseOptions.Section))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddScoped<DomainEventInterceptor>();

            services.AddHostedService<DatabaseInitializer>();

            return services;
        }

        public IServiceCollection AddDatabaseContext<T>(string schemaName)
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

            return services;
        }
    }
}
