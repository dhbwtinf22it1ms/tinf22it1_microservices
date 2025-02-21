using Microsoft.EntityFrameworkCore;

namespace Dhbw.ThesisManager.Domain.Data;

using Address = Entities.Address;
using Comment = Entities.Comment;
using InCompanySupervisor = Entities.InCompanySupervisor;
using OperationalLocation = Entities.OperationalLocation;
using PartnerCompany = Entities.PartnerCompany;
using Student = Entities.Student;
using Thesis = Entities.Thesis;

public class ThesisManagerDbContext : DbContext
{
    public ThesisManagerDbContext(DbContextOptions<ThesisManagerDbContext> options) : base(options)
    {
    }

    public DbSet<Student> Students { get; set; }
    public DbSet<Thesis> Theses { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<PartnerCompany> PartnerCompanies { get; set; }
    public DbSet<OperationalLocation> OperationalLocations { get; set; }
    public DbSet<InCompanySupervisor> InCompanySupervisors { get; set; }
    public DbSet<Comment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Student>()
            .HasOne(s => s.Thesis)
            .WithOne(t => t.Student)
            .HasForeignKey<Thesis>(t => t.StudentId);

        modelBuilder.Entity<Thesis>()
            .HasOne(t => t.PartnerCompany)
            .WithOne(p => p.Thesis)
            .HasForeignKey<PartnerCompany>(p => p.ThesisId);

        modelBuilder.Entity<Thesis>()
            .HasOne(t => t.OperationalLocation)
            .WithOne(o => o.Thesis)
            .HasForeignKey<OperationalLocation>(o => o.ThesisId);

        modelBuilder.Entity<Thesis>()
            .HasOne(t => t.InCompanySupervisor)
            .WithOne(s => s.Thesis)
            .HasForeignKey<InCompanySupervisor>(s => s.ThesisId);

        modelBuilder.Entity<Thesis>()
            .HasMany(t => t.Comments)
            .WithOne(c => c.Thesis)
            .HasForeignKey(c => c.ThesisId);

        modelBuilder.Entity<PartnerCompany>()
            .HasOne(p => p.Address)
            .WithOne()
            .HasForeignKey<PartnerCompany>(p => p.AddressId);

        modelBuilder.Entity<OperationalLocation>()
            .HasOne(o => o.Address)
            .WithOne()
            .HasForeignKey<OperationalLocation>(o => o.AddressId);

        modelBuilder.Entity<Thesis>()
            .Property(t => t.ExcludeSupervisorsFromCompanies)
            .HasColumnType("jsonb");
    }
}
