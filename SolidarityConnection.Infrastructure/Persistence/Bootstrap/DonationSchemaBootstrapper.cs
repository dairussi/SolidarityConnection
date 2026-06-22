using Microsoft.EntityFrameworkCore;

namespace SolidarityConnection.Infrastructure.Persistence.Bootstrap;

public static class DonationSchemaBootstrapper
{
    public static Task EnsureDonationsTableAsync(AppDbContext context)
    {
        const string sql = """
            IF OBJECT_ID(N'[Donations]', N'U') IS NULL
            BEGIN
                CREATE TABLE [Donations] (
                    [Id] uniqueidentifier NOT NULL,
                    [CampaignId] uniqueidentifier NOT NULL,
                    [DonorId] int NOT NULL,
                    [Amount] decimal(18,2) NOT NULL,
                    [Status] nvarchar(20) NOT NULL,
                    [CreatedAt] datetime2 NOT NULL,
                    [ProcessedAt] datetime2 NULL,
                    CONSTRAINT [PK_Donations] PRIMARY KEY ([Id]),
                    CONSTRAINT [CK_Donation_Amount] CHECK ([Amount] > 0),
                    CONSTRAINT [CK_Donation_DonorId] CHECK ([DonorId] > 0),
                    CONSTRAINT [FK_Donations_Campaigns_CampaignId] FOREIGN KEY ([CampaignId]) REFERENCES [Campaigns]([Id]),
                    CONSTRAINT [FK_Donations_Users_DonorId] FOREIGN KEY ([DonorId]) REFERENCES [Users]([Id])
                );

                CREATE INDEX [IX_Donations_CampaignId] ON [Donations] ([CampaignId]);
                CREATE INDEX [IX_Donations_DonorId] ON [Donations] ([DonorId]);
                CREATE INDEX [IX_Donations_Status] ON [Donations] ([Status]);
            END
            """;

        return context.Database.ExecuteSqlRawAsync(sql);
    }
}
