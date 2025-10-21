using Newtonsoft.Json;
using Skycamp.Contracts.Events;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Skycamp.ApiService.Data.Messaging;

[Table("OutboxMessages", Schema = "messaging")]
public class OutboxMessage
{
    [Key]
    public Guid Id { get; set; }

    public string Type { get; set; } = null!;

    public string Payload { get; set; } = null!;

    public DateTime OccurredOnUtc { get; set; } = DateTime.UtcNow;

    public DateTime? ProcessedOnUtc { get; set; }

    public static OutboxMessage Create<TEvent, TId>(TEvent @event) 
        where TEvent : EventBase<TId>
    {
        return new OutboxMessage
        {
            Id = @event.EventId,
            Type = typeof(TEvent).FullName!,
            Payload = JsonConvert.SerializeObject(@event),
            OccurredOnUtc = DateTime.UtcNow
        };
    }

    public static OutboxMessage CreateWithGuidId<TEvent>(TEvent @event)
        where TEvent : EventBase<Guid>
    {
        return Create<TEvent, Guid>(@event);
    }

    public static OutboxMessage CreateWithIntId<TEvent>(TEvent @event)
        where TEvent : EventBase<int>
    {
        return Create<TEvent, int>(@event);
    }

    public static OutboxMessage CreateWithLongId<TEvent>(TEvent @event)
        where TEvent : EventBase<long>
    {
        return Create<TEvent, long>(@event);
    }

    public static OutboxMessage CreateWithStringId<TEvent>(TEvent @event)
        where TEvent : EventBase<string>
    {
        return Create<TEvent, string>(@event);
    }
}