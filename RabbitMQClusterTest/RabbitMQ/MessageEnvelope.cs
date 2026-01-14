using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ
{
    public sealed class MessageEnvelope<T>
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public DateTime TimestampUtc { get; init; } = DateTime.UtcNow;
        public T Payload { get; init; } = default!;
    }
}
