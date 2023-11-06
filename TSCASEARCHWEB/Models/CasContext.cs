using Microsoft.EntityFrameworkCore;

namespace TSCASEARCHWEB.Models;

public partial class CasContext : DbContext
{
    public CasContext()
    {
    }

    public CasContext(DbContextOptions<CasContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Ca> Cas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var builder = WebApplication.CreateBuilder();
        builder.Configuration.AddUserSecrets<Program>();
        var conStrBuilder = builder.Configuration.GetConnectionString("casString");
        //var connection = conStrBuilder.ConnectionString;
        //string? sqlConnection = builder.Configuration["casString"];
        //optionsBuilder.UseSqlite("Data Source=C:\\Users\\charl\\OneDrive\\Documents\\GitHub\\CSharp\\TSCASEARCHWEB\\wwwroot\\CAS.db");
        optionsBuilder.UseSqlite("Data Source=C:\\home\\site\\wwwroot\\wwwroot\\CAS.db");
        //optionsBuilder.UseSqlite(builder.Configuration.GetConnectionString("casString"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ca>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("CAS");

            entity.Property(e => e.Activity).HasColumnName("ACTIVITY");
            entity.Property(e => e.Casregno).HasColumnName("casregno");
            entity.Property(e => e.Casrn).HasColumnName("CASRN");
            entity.Property(e => e.Def).HasColumnName("DEF");
            entity.Property(e => e.Exp).HasColumnName("EXP");
            entity.Property(e => e.Flag).HasColumnName("FLAG");
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Uid).HasColumnName("UID");
            entity.Property(e => e.Uvcb).HasColumnName("UVCB");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
