using Microsoft.Extensions.Logging;
using Quartz;
using SolidarityConnection.Application.Common.Interfaces;

namespace SolidarityConnection.Infrastructure.HostedServices;

[DisallowConcurrentExecution]
public sealed class PendingDonationReprocessingJob(
    IDonationRepository donationRepository,
    IDonationPaymentDispatcher donationPaymentDispatcher,
    ILogger<PendingDonationReprocessingJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var startedAtUtc = DateTime.UtcNow;
        var totalPending = 0;
        var dispatchedCount = 0;
        var failedCount = 0;

        logger.LogInformation(
            "Iniciando execução do worker de reprocessamento de doações pendentes às {StartedAtUtc}.",
            startedAtUtc);

        try
        {
            var pendingDonations = await donationRepository.ListPendingAsync(context.CancellationToken);
            totalPending = pendingDonations.Count;

            foreach (var donation in pendingDonations)
            {
                context.CancellationToken.ThrowIfCancellationRequested();

                try
                {
                    logger.LogInformation(
                        "Worker de reprocessamento reenviando doação pendente {DonationId} para pagamento. CampaignId: {CampaignId}, DonorId: {DonorId}, Amount: {Amount}.",
                        donation.Id,
                        donation.CampaignId,
                        donation.DonorId,
                        donation.Amount);

                    await donationPaymentDispatcher.DispatchAsync(donation, context.CancellationToken);
                    dispatchedCount++;
                }
                catch (Exception exception)
                {
                    failedCount++;

                    logger.LogError(
                        exception,
                        "Erro ao reenviar a doação pendente {DonationId} para o worker de pagamento.",
                        donation.Id);
                }
            }
        }
        finally
        {
            logger.LogInformation(
                "Finalizando execução do worker de reprocessamento de doações pendentes às {FinishedAtUtc}. Pendentes encontradas: {TotalPending}. Reenfileiradas: {DispatchedCount}. Falhas: {FailedCount}.",
                DateTime.UtcNow,
                totalPending,
                dispatchedCount,
                failedCount);
        }
    }
}
