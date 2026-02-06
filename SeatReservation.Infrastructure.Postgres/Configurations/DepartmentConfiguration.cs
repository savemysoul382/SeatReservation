// SeatReservation.Infrastructure.Postgres

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SeatReservation.Domain;
using Path = SeatReservation.Domain.Path;

namespace SeatReservation.Infrastructure.Postgres.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("departments");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .IsRequired()
            .HasColumnName("id")
            .HasConversion(
                value => value.Value,
                value => new DepartmentId(value));

        builder.Property(d => d.Name)
            .IsRequired()
            .HasColumnName("name")
            .HasMaxLength(DepartmentName.NAME_MAX_LENGTH)
            .HasConversion(
                value => value.Value,
                value => DepartmentName.Create(value));

        builder.Property(d => d.Identifier)
            .IsRequired()
            .HasColumnName("identifier")
            .HasMaxLength(Identifier.IDENTIFIER_MAX_LENGTH)
            .HasConversion(
                value => value.Value,
                value => Identifier.Create(value));

        builder.Property(d => d.ParentId)
            .IsRequired(false)
            .HasColumnName("parent_id")
            .HasConversion(
                value => value!.Value,
                value => new DepartmentId(value));
        ;

        builder.Property(d => d.Path)
            .IsRequired()
            .HasColumnName("path")
            .HasColumnType("ltree")
            .HasConversion(
                value => value.Value,
                value => Path.Create(value));

        builder.HasIndex(x => x.Path)
            .HasMethod("gist")
            .HasDatabaseName("idx_departments_path");

        builder.Property(d => d.Depth)
            .IsRequired()
            .HasColumnName("depth");

        builder.Property(d => d.IsActive)
            .IsRequired()
            .HasColumnName("is_active");

        builder.Property(d => d.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(d => d.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at");

        builder.HasMany<Department>()
            .WithOne()
            .HasForeignKey(d => d.ParentId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        // builder.HasMany(d => d.Locations)
        //    .WithOne()
        //    .HasForeignKey(d => d.DepartmentId);
        // builder.HasMany(d => d.DepartmentPositions)
        //    .WithOne()
        //    .HasForeignKey(d => d.DepartmentId);
    }
}