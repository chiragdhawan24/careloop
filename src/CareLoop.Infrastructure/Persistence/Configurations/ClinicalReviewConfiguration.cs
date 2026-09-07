using CareLoop.Domain.Reviews;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareLoop.Infrastructure.Persistence.Configurations;

internal sealed class ClinicalReviewConfiguration
    : IEntityTypeConfiguration<ClinicalReview>
{
    public void Configure(EntityTypeBuilder<ClinicalReview> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.ReviewedBy)
            .IsRequired();

        builder.Property(x => x.ReviewedAt)
            .IsRequired();

        builder.Property(x => x.Notes)
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property<Guid>("CareCaseId")
            .IsRequired();
    }
}