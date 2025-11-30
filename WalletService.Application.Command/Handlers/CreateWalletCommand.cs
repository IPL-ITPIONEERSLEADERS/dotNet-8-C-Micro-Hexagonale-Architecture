using WalletService.Domain.Aggregates;
using WalletService.Domain.ValueObjects;
using WalletService.Infrastructure.EventStore.Contracts;

namespace WalletService.Application.Command.Handlers
{
    public record CreateWalletCommand(Guid OwnerId, decimal InitialBalance);

    public class CreateWalletCommandHandler
    {
        private readonly IEventStore _eventStore;

        public CreateWalletCommandHandler(IEventStore eventStore) => _eventStore = eventStore;

        public async Task<WalletId> Handle(CreateWalletCommand command)
        {
            var wallet = WalletAggregate.Create(new OwnerId(command.OwnerId), command.InitialBalance);
            await _eventStore.SaveEventsAsync(wallet.Id, wallet.GetUncommittedEvents());
            return wallet.Id;
        }
    }
}
