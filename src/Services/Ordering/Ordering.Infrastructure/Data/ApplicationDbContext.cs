using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System.Reflection;
using System.Text;

namespace Ordering.Infrastructure.Data;
public class ApplicationDbContext : DbContext
{
    private readonly ILogger<ApplicationDbContext> _logger;

    private int _saveChangesRetryCount = 3;
    private const int UTC = -3;
    public const string SCHEMA = "";


    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ILogger<ApplicationDbContext> logger)
        : base(options)
    {
        _logger = logger;
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema(SCHEMA);
        //builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }



    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            foreach (var entity in ChangeTracker.Entries<IEntity>())
            {
                switch (entity.State)
                {
                    case EntityState.Added:
                         entity.Entity.CreatedAt = DateTime.UtcNow.AddHours(UTC);
                        break;
                    case EntityState.Modified:
                         entity.Entity.LastModified = DateTime.UtcNow.AddHours(UTC);
                        break;
                    case EntityState.Deleted:
                       //  entity.Entity.DeleteAt = DateTime.UtcNow.AddHours(UTC);
                        break;
                    case EntityState.Detached:
                        break;
                    case EntityState.Unchanged:
                        break;
                    default:
                        break;
                }
            }
            return await base.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            if (this is DbContext dbContext)
            {
                var entries = dbContext.ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified ||
                                e.State == EntityState.Deleted).ToList();

                var sb = new StringBuilder();
                entries.ForEach(entry =>
                {
                    sb.AppendLine($"EntityType: {entry.Metadata.Name} / State: {entry.State} /=/ ");
                });

                _logger.LogCritical(exception,
                    $"{nameof(SaveChanges)} CONCURRENCY DB INSERT/UPDATE/DELETE WARNING: {exception?.Message} / {sb}",
                    exception);

                throw new DbUpdateConcurrencyException(sb.ToString());
            }

            if (_saveChangesRetryCount < 3)
            {
                _saveChangesRetryCount += 1;
                _logger.LogCritical($"SAVECHANGES-RETRY.......: {_saveChangesRetryCount}");
                return await SaveChangesAsync(cancellationToken);
            }

            LogReceivedMessage(LogLevel.Warning, _saveChangesRetryCount, $"SAVECHANGES-RETRY......: {_saveChangesRetryCount} FINAL");
            return 0;
        }
        catch (DbUpdateException exception)
        {
            LogReceivedMessage(LogLevel.Error, _saveChangesRetryCount, $"{nameof(SaveChanges)} DB INSERT/UPDATE/DELETE ERROR: {exception?.Message}");

            throw new DbUpdateException(
                $"{nameof(SaveChanges)} DB INSERT/UPDATE/DELETE ERROR: {exception?.Message}",
                exception);
        }
        catch (RetryLimitExceededException exception)
        {
            throw new RetryLimitExceededException(
                $"{nameof(SaveChanges)} DB INSERT/UPDATE/DELETE ERROR: {exception?.Message}",
                exception);
        }
        catch (Exception exception)
        {
            throw new Exception(
                $"{nameof(SaveChanges)} DB INSERT/UPDATE/DELETE ERROR: {exception?.Message}",
                exception);
        }
    }

    private void LogReceivedMessage<T>(LogLevel logLevel, T exceptionOcurrence, string logMessage, params object[] args)
    {
        using (LogContext.PushProperty("EXCEPTION_OCURRENCE", exceptionOcurrence, true))
            _logger.Log(logLevel, logMessage, args);

    }


}
