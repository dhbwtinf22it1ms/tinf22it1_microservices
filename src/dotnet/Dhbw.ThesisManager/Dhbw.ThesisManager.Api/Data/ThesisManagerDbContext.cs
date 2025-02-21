using Dhbw.ThesisManager.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dhbw.ThesisManager.Api.Data;
using DbEntities = Dhbw.ThesisManager.Api.Data.Entities;
public class ThesisManagerDbContext : DbContext
{
    public ThesisManagerDbContext(DbContextOptions<ThesisManagerDbContext> options) : base(options)
    {
    }

    public DbSet<DbEntities.Student> Students { get; set; }
    public DbSet<DbEntities.Thesis> Theses { get; set; }
    public DbSet<DbEntities.Address> Addresses { get; set; }
    public DbSet<DbEntities.PartnerCompany> PartnerCompanies { get; set; }
    public DbSet<DbEntities.OperationalLocation> OperationalLocations { get; set; }
    public DbSet<DbEntities.InCompanySupervisor> InCompanySupervisors { get; set; }
    public DbSet<DbEntities.Comment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DbEntities.Student>()
            .HasOne(s => s.Thesis)
            .WithOne(t => t.Student)
            .HasForeignKey<DbEntities.Thesis>(t => t.StudentId);

        modelBuilder.Entity<DbEntities.Thesis>()
            .HasOne(t => t.PartnerCompany)
            .WithOne(p => p.Thesis)
            .HasForeignKey<DbEntities.PartnerCompany>(p => p.ThesisId);

        modelBuilder.Entity<DbEntities.Thesis>()
            .HasOne(t => t.OperationalLocation)
            .WithOne(o => o.Thesis)
            .HasForeignKey<DbEntities.OperationalLocation>(o => o.ThesisId);

        modelBuilder.Entity<DbEntities.Thesis>()
            .HasOne(t => t.InCompanySupervisor)
            .WithOne(s => s.Thesis)
            .HasForeignKey<DbEntities.InCompanySupervisor>(s => s.ThesisId);

        modelBuilder.Entity<DbEntities.Thesis>()
            .HasMany(t => t.Comments)
            .WithOne(c => c.Thesis)
            .HasForeignKey(c => c.ThesisId);

        modelBuilder.Entity<DbEntities.PartnerCompany>()
            .HasOne(p => p.Address)
            .WithOne()
            .HasForeignKey<DbEntities.PartnerCompany>(p => p.AddressId);

        modelBuilder.Entity<DbEntities.OperationalLocation>()
            .HasOne(o => o.Address)
            .WithOne()
            .HasForeignKey<DbEntities.OperationalLocation>(o => o.AddressId);

        modelBuilder.Entity<DbEntities.Thesis>()
            .Property(t => t.ExcludeSupervisorsFromCompanies)
            .HasColumnType("jsonb");
    }
}
