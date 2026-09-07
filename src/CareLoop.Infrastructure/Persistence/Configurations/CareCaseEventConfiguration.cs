using CareLoop.Domain.CareCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareLoop.Infrastructure.Persistence.Configurations;

internal sealed class CareCaseEventConfiguration
    : IEntityTypeConfiguration<CareCaseEvent>
{
    public void Configure(EntityTypeBuilder<CareCaseEvent> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Type)
            .HasConversion<int>();

        builder.Property(x => x.OccurredAt).IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property<Guid>("CareCaseId").IsRequired();
    }
}