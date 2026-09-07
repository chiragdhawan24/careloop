using CareLoop.Domain.FollowUps;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareLoop.Infrastructure.Persistence.Configurations;

internal sealed class FollowUpActionConfiguration
    : IEntityTypeConfiguration<FollowUpAction>
{
    public void Configure(EntityTypeBuilder<FollowUpAction> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Description)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.AssignedTo).IsRequired();
        builder.Property(x => x.DueAt).IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<int>();

        builder.Property<Guid>("CareCaseId").IsRequired();

        builder.HasIndex(x => new { x.Status, x.DueAt });
    }
}