using Newtonsoft.Json;

namespace Skycamp.Contracts.Events;

public abstract class EventBase<T>
{
    [JsonProperty("$Version")]
    public abstract int Version { get; }

    public abstract T Id { get; init; }

    [JsonProperty("$Eventid")]
    public Guid EventId { get; init; } = Guid.CreateVersion7();

    [JsonProperty("$Type")]
    public string Type { get; init; }

    protected EventBase()
    {
        Type = GetType().Name;
    }
}