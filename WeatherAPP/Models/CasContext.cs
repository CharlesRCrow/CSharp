using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

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
        //=> optionsBuilder.UseSqlite("Name=ConnectionsString:CasString");
        => optionsBuilder.UseSqlite("Data Source=/workspaces/CSharp/WeatherAPP/wwwroot/CAS.db");

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
