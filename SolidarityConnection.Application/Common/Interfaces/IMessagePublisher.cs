using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidarityConnection.Application.Common.Interfaces;
public interface IMessagePublisher
{
    Task PublishAsync<T>(T message,string queueName,CancellationToken cancellationToken);
}
