using Microsoft.EntityFrameworkCore;
using CSharpPizza.Data.Entities;

namespace CSharpPizza.Data;

public class PizzaDbContext : DbContext
{
    public PizzaDbContext(DbContextOptions<PizzaDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Pizza> Pizzas => Set<Pizza>();
    public DbSet<Topping> Toppings => Set<Topping>();
    public DbSet<PizzaTopping> PizzaToppings => Set<PizzaTopping>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<CartItemTopping> CartItemToppings => Set<CartItemTopping>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Log> Logs => Set<Log>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=pizza.db");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure soft-delete query filter for all entities inheriting from BaseEntity
        modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
        modelBuilder.Entity<Pizza>().HasQueryFilter(p => !p.IsDeleted);
        modelBuilder.Entity<Topping>().HasQueryFilter(t => !t.IsDeleted);
        modelBuilder.Entity<Cart>().HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.Entity<CartItem>().HasQueryFilter(ci => !ci.IsDeleted);
        modelBuilder.Entity<Order>().HasQueryFilter(o => !o.IsDeleted);
        modelBuilder.Entity<OrderItem>().HasQueryFilter(oi => !oi.IsDeleted);
        modelBuilder.Entity<Log>().HasQueryFilter(l => !l.IsDeleted);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Name).IsRequired();
            entity.Property(u => u.Email).IsRequired();
            entity.Property(u => u.Mobile).IsRequired();
            entity.Property(u => u.Address).IsRequired();
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.UserRole).HasConversion<int>();
        });

        // Pizza configuration
        modelBuilder.Entity<Pizza>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired();
            entity.Property(p => p.Description).IsRequired();
            entity.Property(p => p.BasePrice).HasColumnType("decimal(18,2)");
            entity.Property(p => p.Slug).IsRequired();
            entity.HasIndex(p => p.Slug).IsUnique();
        });

        // Topping configuration
        modelBuilder.Entity<Topping>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Name).IsRequired();
            entity.Property(t => t.Description).IsRequired();
            entity.Property(t => t.Cost).HasColumnType("decimal(18,2)");
        });

        // PizzaTopping (many-to-many) configuration
        modelBuilder.Entity<PizzaTopping>(entity =>
        {
            entity.HasKey(pt => new { pt.PizzaId, pt.ToppingId });
            
            entity.HasOne(pt => pt.Pizza)
                .WithMany(p => p.PizzaToppings)
                .HasForeignKey(pt => pt.PizzaId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(pt => pt.Topping)
                .WithMany(t => t.PizzaToppings)
                .HasForeignKey(pt => pt.ToppingId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Cart configuration
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.HasIndex(c => c.UserId).IsUnique();
            
            entity.HasOne(c => c.User)
                .WithOne(u => u.Cart)
                .HasForeignKey<Cart>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CartItem configuration
        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(ci => ci.Id);
            entity.Property(ci => ci.Quantity).IsRequired();
            
            entity.HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(ci => ci.Pizza)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.PizzaId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CartItemTopping configuration
        modelBuilder.Entity<CartItemTopping>(entity =>
        {
            entity.HasKey(cit => new { cit.CartItemId, cit.ToppingId });
            entity.Property(cit => cit.IsAdded).IsRequired();
            
            entity.HasOne(cit => cit.CartItem)
                .WithMany(ci => ci.CartItemToppings)
                .HasForeignKey(cit => cit.CartItemId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(cit => cit.Topping)
                .WithMany(t => t.CartItemToppings)
                .HasForeignKey(cit => cit.ToppingId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Order configuration
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.Id);
            entity.Property(o => o.Status).HasConversion<int>();
            entity.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");
            
            entity.HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // OrderItem configuration
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(oi => oi.Id);
            entity.Property(oi => oi.PizzaName).IsRequired();
            entity.Property(oi => oi.PizzaPrice).HasColumnType("decimal(18,2)");
            entity.Property(oi => oi.Quantity).IsRequired();
            entity.Property(oi => oi.Toppings).IsRequired();
            
            entity.HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Log configuration
        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(l => l.Id);
            entity.Property(l => l.LogLevel).IsRequired();
            entity.Property(l => l.Message).IsRequired();
        });
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}