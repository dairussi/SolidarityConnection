using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolidarityConnection.Domain.Campaign.Models;
using SolidarityConnection.Domain.Donation.Models;
using SolidarityConnection.Domain.User.Models;

namespace SolidarityConnection.Infrastructure.Persistence.Configurations;

public sealed class DonationConfiguration : IEntityTypeConfiguration<Donation>
{
    public void Configure(EntityTypeBuilder<Donation> entity)
    {
        entity.ToTable("Donations", table =>
        {
            table.HasCheckConstraint("CK_Donation_Amount", "[Amount] > 0");
            table.HasCheckConstraint("CK_Donation_DonorId", "[DonorId] > 0");
        });

        entity.HasKey(donation => donation.Id);

        entity.Property(donation => donation.Id)
            .HasColumnType("uniqueidentifier")
            .ValueGeneratedNever();

        entity.Property(donation => donation.CampaignId)
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        entity.Property(donation => donation.DonorId)
            .HasColumnType("int")
            .IsRequired();

        entity.Property(donation => donation.Amount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        entity.Property(donation => donation.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        entity.Property(donation => donation.CreatedAt)
            .HasColumnType("datetime2")
            .IsRequired();

        entity.Property(donation => donation.ProcessedAt)
            .HasColumnType("datetime2");

        entity.HasIndex(donation => donation.CampaignId);
        entity.HasIndex(donation => donation.DonorId);
        entity.HasIndex(donation => donation.Status);

        entity.HasOne<Campaign>()
            .WithMany()
            .HasForeignKey(donation => donation.CampaignId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne<User>()
            .WithMany()
            .HasForeignKey(donation => donation.DonorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
