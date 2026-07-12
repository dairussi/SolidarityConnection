using Moq;
using Quartz;
using SolidarityConnection.Application.Common.Interfaces;
using SolidarityConnection.Domain.Donation.Models;
using SolidarityConnection.Infrastructure.HostedServices;

namespace SolidarityConnection.Tests.Infrastructure.HostedServices;

public class PendingDonationReprocessingJobTests
{
    [Fact]
    public async Task Execute_ShouldDispatchAllPendingDonations()
    {
        var pendingDonations = new[]
        {
            Donation.Create(Guid.NewGuid(), 10, 25),
            Donation.Create(Guid.NewGuid(), 20, 50)
        };

        var donationRepositoryMock = new Mock<IDonationRepository>();
        var donationPaymentDispatcherMock = new Mock<IDonationPaymentDispatcher>();
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<PendingDonationReprocessingJob>>();
        var contextMock = new Mock<IJobExecutionContext>();

        donationRepositoryMock
            .Setup(repository => repository.ListPendingAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(pendingDonations);

        contextMock
            .SetupGet(context => context.CancellationToken)
            .Returns(CancellationToken.None);

        var job = new PendingDonationReprocessingJob(
            donationRepositoryMock.Object,
            donationPaymentDispatcherMock.Object,
            loggerMock.Object);

        await job.Execute(contextMock.Object);

        donationPaymentDispatcherMock.Verify(
            dispatcher => dispatcher.DispatchAsync(It.IsAny<Donation>(), CancellationToken.None),
            Times.Exactly(pendingDonations.Length));
    }

    [Fact]
    public async Task Execute_ShouldSkipDispatch_WhenThereAreNoPendingDonations()
    {
        var donationRepositoryMock = new Mock<IDonationRepository>();
        var donationPaymentDispatcherMock = new Mock<IDonationPaymentDispatcher>();
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<PendingDonationReprocessingJob>>();
        var contextMock = new Mock<IJobExecutionContext>();

        donationRepositoryMock
            .Setup(repository => repository.ListPendingAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Donation>());

        contextMock
            .SetupGet(context => context.CancellationToken)
            .Returns(CancellationToken.None);

        var job = new PendingDonationReprocessingJob(
            donationRepositoryMock.Object,
            donationPaymentDispatcherMock.Object,
            loggerMock.Object);

        await job.Execute(contextMock.Object);

        donationPaymentDispatcherMock.Verify(
            dispatcher => dispatcher.DispatchAsync(It.IsAny<Donation>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
