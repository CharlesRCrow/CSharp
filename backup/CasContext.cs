using System.Collections.Immutable;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace WeatherAPP.Models;

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
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlite("Data Source=/workspaces/CSharp/WeatherAPP/CAS.db");
        //=> optionsBuilder.UseSqlite(System.Configuration.GetConnectionString("CasString"));
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
