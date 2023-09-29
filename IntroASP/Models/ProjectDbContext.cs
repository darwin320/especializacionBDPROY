using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace IntroASP.Models;

public partial class ProjectDbContext : DbContext
{
    public ProjectDbContext()
    {
    }

    public ProjectDbContext(DbContextOptions<ProjectDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Attendance> Attendances { get; set; }

    public virtual DbSet<Beer> Beers { get; set; }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<RegistryEvent> RegistryEvents { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;database=project_db;user id=root", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.4.27-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.HasKey(e => e.IdAttendance).HasName("PRIMARY");

            entity.ToTable("attendance");

            entity.HasIndex(e => e.CorreoElectronico, "Correo_electronico");

            entity.HasIndex(e => e.IdEvent, "id_event");

            entity.Property(e => e.IdAttendance)
                .HasColumnType("int(11)")
                .HasColumnName("id_attendance");
            entity.Property(e => e.Apellido).HasMaxLength(255);
            entity.Property(e => e.CorreoElectronico).HasColumnName("Correo_electronico");
            entity.Property(e => e.Duracion).HasMaxLength(255);
            entity.Property(e => e.Fecha).HasMaxLength(50);
            entity.Property(e => e.HoraSalio)
                .HasColumnType("text")
                .HasColumnName("Hora_Salio");
            entity.Property(e => e.HoraUnio)
                .HasColumnType("text")
                .HasColumnName("Hora_Unio");
            entity.Property(e => e.IdEvent)
                .HasColumnType("int(11)")
                .HasColumnName("id_event");
            entity.Property(e => e.Nombre).HasMaxLength(255);

            entity.HasOne(d => d.IdEventNavigation).WithMany(p => p.Attendances)
                .HasForeignKey(d => d.IdEvent)
                .HasConstraintName("attendance_ibfk_1");
        });

        modelBuilder.Entity<Beer>(entity =>
        {
            entity.HasKey(e => e.BeerId).HasName("PRIMARY");

            entity.ToTable("beer");

            entity.HasIndex(e => e.BrandId, "BrandId");

            entity.Property(e => e.BeerId).HasColumnType("int(11)");
            entity.Property(e => e.BrandId).HasColumnType("int(11)");
            entity.Property(e => e.Name).HasMaxLength(255);

            entity.HasOne(d => d.Brand).WithMany(p => p.Beers)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("beer_ibfk_1");
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.BrandId).HasName("PRIMARY");

            entity.ToTable("brand");

            entity.Property(e => e.BrandId).HasColumnType("int(11)");
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<RegistryEvent>(entity =>
        {
            entity.HasKey(e => e.IdEvent).HasName("PRIMARY");

            entity.ToTable("registry_event");

            entity.Property(e => e.IdEvent)
                .HasColumnType("int(11)")
                .HasColumnName("id_event");
            entity.Property(e => e.Asignatura).HasMaxLength(255);
            entity.Property(e => e.CorreoInstitucional)
                .HasMaxLength(255)
                .HasColumnName("Correo_Institucional");
            entity.Property(e => e.Nombre).HasMaxLength(255);
            entity.Property(e => e.ProgramaAcademico)
                .HasMaxLength(255)
                .HasColumnName("Programa_Academico");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
