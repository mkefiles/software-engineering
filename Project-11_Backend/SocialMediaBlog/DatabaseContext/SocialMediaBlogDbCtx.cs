using Microsoft.EntityFrameworkCore;
using SocialMediaBlog.Account;
using SocialMediaBlog.Messages;

namespace SocialMediaBlog.DatabaseContext;

public class SocialMediaBlogDbCtx : DbContext {
    // NOTE: This is a Constructor that, when an instance is created,
    // ... calls the base-class constructor first
    // NOTE: The `options` parameter is a class that holds config.
    // ... settings for the `DbContext` -- this, with `base(options)`
    // ... is a mechanism to pass config. settings that are managed
    // ... by the "Options Pattern"
    public SocialMediaBlogDbCtx(
        DbContextOptions options) : base(options) { }
    
    // NOTE: `DbSet` is a collection of all <T> Entities
    // NOTE: Using the "Null-Forgiving Operator", this sets the
    // ... initial value of DbSet to `null` knowing that the
    // ... value will be set to a non-null value prior to being called
    // NOTE: This is necessary because EFC requires `DbSet<T>` to
    // ... be non-null and EFC uses "reflection" to instantiate
    // ... this at run-time
    public DbSet<AccountEntity> Accounts { get; set; } = null!;
    public DbSet<MessagesEntity> Messages { get; set; } = null!;
    
    // NOTE: PT03 / 03 - This is added to enforce a one-to-many
    // ... Foreign Key relationship between `accountId` and `postedBy`
    /* learn.microsoft.com/en-us/ef/core/modeling/relationships/foreign-and-principal-keys */
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<AccountEntity>()
            .HasMany(accountEntity => accountEntity.MessageEntities)
            .WithOne(messageEntity => messageEntity.Account)
            .HasForeignKey(messageEntity => messageEntity.PostedBy)
            .IsRequired();
    }
}