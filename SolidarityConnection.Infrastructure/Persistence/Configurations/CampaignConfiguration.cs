using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolidarityConnection.Domain.Campaign.Models;
using SolidarityConnection.Domain.User.Models;

namespace SolidarityConnection.Infrastructure.Persistence.Configurations;

public sealed class CampaignConfiguration : IEntityTypeConfiguration<Campaign>
{
    public void Configure(EntityTypeBuilder<Campaign> entity)
    {
        entity.ToTable("Campaigns", table =>
        {
            table.HasCheckConstraint("CK_Campaign_TargetAmount", "[TargetAmount] > 0");
            table.HasCheckConstraint("CK_Campaign_AmountRaised", "[AmountRaised] >= 0");
            table.HasCheckConstraint("CK_Campaign_Dates", "[EndDate] > [StartDate]");
        });

        entity.HasKey(campaign => campaign.Id);

        entity.Property(campaign => campaign.Id)
            .HasColumnType("uniqueidentifier")
            .ValueGeneratedNever();

        entity.Property(campaign => campaign.Title)
            .HasMaxLength(200)
            .IsRequired();

        entity.Property(campaign => campaign.Description)
            .HasMaxLength(2000)
            .IsRequired();

        entity.Property(campaign => campaign.StartDate)
            .HasColumnType("datetime2")
            .IsRequired();

        entity.Property(campaign => campaign.EndDate)
            .HasColumnType("datetime2")
            .IsRequired();

        entity.Property(campaign => campaign.TargetAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        entity.Property(campaign => campaign.AmountRaised)
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0m)
            .IsRequired();

        entity.Property(campaign => campaign.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        entity.Property(campaign => campaign.ManagerId)
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        entity.HasIndex(campaign => campaign.ManagerId);
        entity.HasIndex(campaign => campaign.Status);

        entity.HasOne<User>()
            .WithMany()
            .HasForeignKey(campaign => campaign.ManagerId)
            .HasPrincipalKey(user => user.PublicId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
