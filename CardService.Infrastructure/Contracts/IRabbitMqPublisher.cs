using Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardService.Infrastructure.Contracts
{
    public interface IRabbitMqPublisher
    {
        Task PublishEventAsync(IDomainEvent @event);
    }
}
