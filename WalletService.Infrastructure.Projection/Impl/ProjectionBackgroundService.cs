using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using WalletService.Domain.Events;
using WalletService.Domain.ValueObjects;
using WalletService.Infrastructure.EventStore.Contracts;
using WalletService.Infrastructure.Projection.Contracts;

namespace WalletService.Infrastructure.Projection.Impl
{
    public class ProjectionBackgroundService : BackgroundService
    {
        private readonly IEventStore _eventStore;
        private readonly IWalletProjection _projection;

        public ProjectionBackgroundService(IEventStore eventStore, IWalletProjection projection)
        {
            _eventStore = eventStore;
            _projection = projection;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _eventStore.SubscribeToAllEvents(async domainEvent =>
            {
                WalletId? walletId = null;
                decimal? newBalance = null;

                Console.WriteLine("Domain Event " + domainEvent.Timestamp);

                switch (domainEvent)
                {
                    case WalletCreated e:
                        walletId = e.WalletId;
                        newBalance = e.InitialBalance;
                        break;
                    case FundsDeposited e:
                        walletId = e.WalletId;
                        // On ne connaît pas le solde → on relit depuis l'Event Store (ou on stocke dans l'événement)
                        // Pour simplifier, on suppose que l'événement contient le nouveau solde → ici, on ne le fait pas
                        // Dans la vraie vie, on relit l'agrégat ou on stocke le solde dans l'événement
                        break;
                    default:
                        return;
                }

                if (walletId != null && newBalance != null)
                    await _projection.UpdateBalance(walletId, newBalance.Value);
            });
        }
    }
}
