using Backend.BankingTranxSystem.DataAccess.Entities;
using Backend.BankingTranxSystem.DataAccess.Enums;
using Backend.BankingTranxSystem.SharedServices.Domain;
using Backend.BankingTranxSystem.SharedServices.Helper;
using Backend.BankingTranxSystem.SharedServices.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Reflection;

namespace Backend.BankingTranxSystem.API.Data;

public class BankingTranxSystemContext(IMediator mediator, /*IDbContextTransaction currentTransaction*/ DbContextOptions options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<WalletTransaction> WalletTransactions { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var modifiedEntries = this.ChangeTracker.Entries()
                   .Where(x => x.State == EntityState.Modified)
                   .Select(x => x.Entity);

        foreach (var modifiedEntry in modifiedEntries)
        {
            //If the updated object inherits from SharedBase
            var auditableEntity = modifiedEntry as SharedBase;
            if (auditableEntity != null)
            {
                auditableEntity.UpdatedAt = DateTime.UtcNow;
            }
        }

        //publish domain events
        var entitiesWithEvents = ChangeTracker
          .Entries()
          .Select(e => e.Entity as BaseEntity<Guid>)
          .Where(e => e?.Events != null && e.Events.Any())
          .ToArray();

        foreach (var entity in entitiesWithEvents)
        {
            var events = entity.Events.ToArray();
            entity.Events.Clear();
            foreach (var domainEvent in events)
            {
                await mediator.Publish(domainEvent, cancellationToken).ConfigureAwait(false);
            }
        }

        //public domain events
        var eventEntitiesWithEvent = ChangeTracker
            .Entries()
            .Select(e => e.Entity as User)
            .Where(e => e?.Events != null && e.Events.Any())
            .ToArray();

        //save changes before publishing any entity event
        var k = await base.SaveChangesAsync(cancellationToken);

        //iterate through events and publish
        foreach (var u in eventEntitiesWithEvent)
        {
            var events = u.Events.ToArray();
            u.Events.Clear();
            foreach (var domainEvent in events)
            {
                await mediator.Publish(domainEvent, cancellationToken).ConfigureAwait(false);
            }
        }

        return k;

        /*
        catch (DbUpdateConcurrencyException ex)
        {
            foreach (var entry in ex.Entries)
            {
                if (entry.Entity is UserAssetBalance)
                {
                    var proposedValues = entry.CurrentValues;
                    var databaseValues = entry.GetDatabaseValues();

                    foreach (var property in proposedValues.Properties)
                    {
                        var proposedValue = proposedValues[property];
                        var databaseValue = databaseValues[property];

                        // TODO: decide which value should be written to database
                        // proposedValues[property] = <value to be saved>;
                    }

                    // Refresh original values to bypass next concurrency check
                    entry.OriginalValues.SetValues(databaseValues);

                    return base.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    throw;
                }
            }

            throw;
        }
        */
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Ignore<BaseDomainEvent>();

        modelBuilder.Entity<User>()
            .HasIndex(c => c.EmailAddress)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(c => c.Password)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(c => c.TelephoneNumber)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(c => c.Bvn)
            .IsUnique();

        modelBuilder.Entity<Wallet>()
            .HasIndex(c => c.Reference)
            .IsUnique();

        modelBuilder.Entity<WalletTransaction>()
            .HasIndex(c => c.SourceReference);


        modelBuilder.Entity<WalletTransaction>()
            .HasIndex(c => c.DestinationReference);


        foreach (var property in modelBuilder.Model.GetEntityTypes()
                    .SelectMany(t => t.GetProperties())
                    .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
        {
            property.SetPrecision(18);
            property.SetScale(8);
        }

        foreach (var property in modelBuilder.Model.GetEntityTypes()
                    .SelectMany(t => t.GetProperties())
                    .Where(p => p.Name == nameof(SharedBase.CreatedAt) || p.Name == nameof(SharedBase.UpdatedAt)))
        {
            property.SetDefaultValueSql("GETUTCDATE()");
        }

       
    }

    //public async Task BeginTransactionAsync()
    //{
    //    if (currentTransaction != null)
    //    {
    //        return;
    //    }

    //    currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
    //}

    //[Obsolete]
    //public async Task CommitTransactionAsync()
    //{
    //    try
    //    {
    //        //in case it wasn't started in the caller
    //        await BeginTransactionAsync();

    //        await SaveChangesAsync();

    //        //if (await PostChecksValid() == false)
    //        //    throw new TransactionRolledBackException("Could not process this request due to negative balance");

    //        await (currentTransaction?.CommitAsync() ?? Task.CompletedTask);
    //    }
    //    catch
    //    {
    //        RollbackTransaction();
    //        throw;
    //    }
    //    finally
    //    {
    //        if (currentTransaction != null)
    //        {
    //            currentTransaction.Dispose();
    //            currentTransaction = null;
    //        }
    //    }
    //}
    //public void RollbackTransaction()
    //{
    //    try
    //    {
    //        currentTransaction?.Rollback();
    //    }
    //    finally
    //    {
    //        if (currentTransaction != null)
    //        {
    //            currentTransaction.Dispose();
    //            currentTransaction = null;
    //        }
    //    }
    //}
}
