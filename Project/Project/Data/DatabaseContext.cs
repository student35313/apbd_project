using Microsoft.AspNetCore.Identity;
using Project.Models;

namespace Project.Data;

using Microsoft.EntityFrameworkCore;


public class DatabaseContext : DbContext
{
    // Auth
    public DbSet<User> Users=> Set<User>();
    public DbSet<UserRole> UserRoles=> Set<UserRole>();
    public DbSet<RefreshToken> RefreshTokens=> Set<RefreshToken>();

    // Business
    public DbSet<Client> Clients=> Set<Client>();
    public DbSet<IndividualClient> Individuals=> Set<IndividualClient>();
    public DbSet<CompanyClient> Companies=> Set<CompanyClient>();
    public DbSet<Product> Products=> Set<Product>();
    public DbSet<Discount> Discounts=> Set<Discount>();
    public DbSet<SoftwareProduct> SoftwareProducts=> Set<SoftwareProduct>();
    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<Payment> Payments => Set<Payment>();


    protected DatabaseContext() { }
    public DatabaseContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<UserRole>().HasData(
            new UserRole { Id = 1, Name = "User"  },
            new UserRole { Id = 2, Name = "Admin" }
        );
        
        modelBuilder.Entity<Client>()
            .HasDiscriminator<string>("ClientType")
            .HasValue<IndividualClient>("Individual")
            .HasValue<CompanyClient>  ("Company");

        //Individual
        modelBuilder.Entity<IndividualClient>()
            .HasIndex(i => i.Pesel)
            .IsUnique();

        // modelBuilder.Entity<Client>()
        //     .HasQueryFilter(c => !c.IsDeleted);
        
        //Company
        modelBuilder.Entity<CompanyClient>()
            .HasIndex(c => c.KrsNumber)
            .IsUnique();
        
        modelBuilder.Entity<Product>()
            .HasDiscriminator<string>("ProductType")
            .HasValue<SoftwareProduct>("Software");
        
        
        
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = 1,
            Username = "admin",
            PasswordHash = "AQAAAAIAAYagAAAAEBYbj1UOyqSRxz7yMASEPPIotzUGbvbUAsfs+Iem/tkinB40Zp+ZMgQbsu3B1jDK7w==",
            RoleId = 2
        });
        
        modelBuilder.Entity<IndividualClient>().HasData(
            new IndividualClient
            {
                Id = 1,
                FirstName = "Jan",
                LastName = "Kowalski",
                Pesel = "90010112345",
                Email = "jan@example.com",
                PhoneNumber = "123456789",
                Address = "Warsaw",
                IsDeleted = false
            }
        );

        modelBuilder.Entity<CompanyClient>().HasData(
            new CompanyClient
            {
                Id = 2,
                CompanyName = "SoftCorp",
                KrsNumber = "0000123456",
                Email = "contact@softcorp.com",
                PhoneNumber = "987654321",
                Address = "Cracow"
            }
        );
        
        modelBuilder.Entity<SoftwareProduct>().HasData(
            new SoftwareProduct
            {
                Id = 1,
                Name = "EduTrack",
                Description = "Educational platform for schools",
                Category = "Education",
                CurrentVersion = "1.2.0",
                SubscriptionPrice = 99,
                UpfrontPrice = null,
                ClientId = 1
            },
            new SoftwareProduct
            {
                Id = 2,
                Name = "FinMate",
                Description = "Finance management software",
                Category = "Finance",
                CurrentVersion = "3.4.1",
                UpfrontPrice = 499,
                SubscriptionPrice = null,
                ClientId = 1
            }
        );


    }
}
