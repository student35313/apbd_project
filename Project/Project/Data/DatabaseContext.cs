using Project.Models;

namespace Project.Data;

using Microsoft.EntityFrameworkCore;


public class DatabaseContext : DbContext
{
    // Auth
    public DbSet<User>         Users          => Set<User>();
    public DbSet<UserRole>     UserRoles      => Set<UserRole>();
    public DbSet<RefreshToken> RefreshTokens  => Set<RefreshToken>();

    // Business
    public DbSet<Client>            Clients           => Set<Client>();
    public DbSet<IndividualClient>  Individuals       => Set<IndividualClient>();
    public DbSet<CompanyClient>     Companies         => Set<CompanyClient>();

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

        modelBuilder.Entity<Client>()
            .HasQueryFilter(c => !c.IsDeleted);



        //Company
        modelBuilder.Entity<CompanyClient>()
            .HasIndex(c => c.KrsNumber)
            .IsUnique();
    }
}
