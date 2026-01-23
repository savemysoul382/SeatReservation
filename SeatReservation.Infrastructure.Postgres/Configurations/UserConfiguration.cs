// SeatReservation.Infrastructure.Postgres

using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SeatReservation.Domain;

namespace SeatReservation.Infrastructure.Postgres.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(v => v.Id).HasName("pk_users");

        builder.Property(v => v.Id)
            .HasColumnName("id");

        // builder.Property(u => u.Socials)
        //    .HasColumnType("jsonb");
        // builder.OwnsMany(
        //    u => u.Socials,
        //    sb =>
        //    {
        //        sb.ToJson("socials");
        //        sb.Property(u => u.Link)
        //            .IsRequired()
        //            .HasMaxLength(LengthConstants.LENGTH500)
        //            .HasColumnName("link");
        //        sb.Property(u => u.Name)
        //            .IsRequired()
        //            .HasMaxLength(LengthConstants.LENGTH500)
        //            .HasColumnName("name");
        //    });

        // можно так, но медленнее, но не можем более подробно сконфигурировать link, name - required? maxLength и прочее
        // builder.Property(u => u.Details)
        //    .HasConversion(
        //        v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
        //        json => JsonSerializer.Deserialize<Details>(json, JsonSerializerOptions.Default)! )
        //    .HasColumnType("jsonb");

        // ComplexProperty с коллекциями глючит
        builder.OwnsOne(
            u => u.Details,
            db =>
            {
                db.ToJson("details");

                db.OwnsMany(
                    d => d.Socials,
                    sb =>
                    {
                        sb.Property(u => u.Link).IsRequired().HasMaxLength(LengthConstants.LENGTH500).HasColumnName("link");
                        sb.Property(u => u.Name).IsRequired().HasMaxLength(LengthConstants.LENGTH500).HasColumnName("name");
                    });

                db.Property(u => u.Description).IsRequired().HasMaxLength(LengthConstants.LENGTH500).HasColumnName("description");
            });
    }
}