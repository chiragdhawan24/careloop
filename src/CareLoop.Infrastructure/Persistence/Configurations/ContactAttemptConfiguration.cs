using CareLoop.Domain.Communications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareLoop.Infrastructure.Persistence.Configurations;

internal sealed class ContactAttemptConfiguration
    : IEntityTypeConfiguration<ContactAttempt>
{
    public void Configure(EntityTypeBuilder<ContactAttempt> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Method)
            .HasConversion<int>();

        builder.Property(x => x.AttemptedAt).IsRequired();
        builder.Property(x => x.Successful).IsRequired();

        builder.Property(x => x.Notes)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property<Guid>("CareCaseId").IsRequired();
    }
}