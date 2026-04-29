using Microsoft.EntityFrameworkCore;
using PoliceManagementSystem.Data;
using PoliceManagementSystem.Services.Interfaces;

namespace PoliceManagementSystem.BackgroundServices
{
    /// <summary>Background service that automatically reverts expired temporary transfers (REQ-70).</summary>
    public class TransferExpiryService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<TransferExpiryService> _logger;
        private static readonly TimeSpan CheckInterval = TimeSpan.FromHours(1);

        /// <summary>Initializes a new instance of TransferExpiryService.</summary>
        /// <param name="scopeFactory">Factory for creating DI scopes.</param>
        /// <param name="logger">Logger instance.</param>
        public TransferExpiryService(IServiceScopeFactory scopeFactory,
                                     ILogger<TransferExpiryService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        /// <summary>Executes the background service loop.</summary>
        /// <param name="stoppingToken">Cancellation token for graceful shutdown.</param>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("TransferExpiryService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                await RevertExpiredTransfersAsync();
                await Task.Delay(CheckInterval, stoppingToken);
            }

            _logger.LogInformation("TransferExpiryService stopped.");
        }

        /// <summary>Finds expired temporary transfers and reverts agents to original stations.</summary>
        private async Task RevertExpiredTransfersAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var auditLogging = scope.ServiceProvider.GetRequiredService<IAuditLoggingService>();

            var expiredTransfers = await context.AgentTransfers
                .Include(at => at.Agent)
                .Where(at => !at.IsPermanent
                          && at.IsApproved
                          && at.EndDate.HasValue
                          && at.EndDate.Value <= DateTime.UtcNow)
                .ToListAsync();

            foreach (var transfer in expiredTransfers)
            {
                _logger.LogInformation(
                    "Reverting agent {AgentId} from station {ToStation} back to {FromStation}.",
                    transfer.AgentId, transfer.ToStationId, transfer.FromStationId);

                transfer.Agent.PoliceStationId = transfer.FromStationId;

                await auditLogging.LogAsync(
                    action: "TRANSFER_REVERTED",
                    entityType: "AgentTransfer",
                    entityId: transfer.Id.ToString(),
                    oldValue: $"StationId={transfer.ToStationId}",
                    newValue: $"StationId={transfer.FromStationId}"
                );

                context.AgentTransfers.Remove(transfer);
            }

            if (expiredTransfers.Count > 0)
                await context.SaveChangesAsync();
        }
    }
}