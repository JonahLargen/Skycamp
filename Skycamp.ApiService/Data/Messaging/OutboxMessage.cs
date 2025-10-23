using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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

    public static OutboxMessage Create<TEvent>(TEvent @event)
    {
        return new OutboxMessage
        {
            Type = typeof(TEvent).FullName!,
            Payload = JsonConvert.SerializeObject(@event, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }),
            OccurredOnUtc = DateTime.UtcNow
        };
    }
}

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.Property(m => m.Id)
            .HasValueGenerator<SequentialGuidValueGenerator>();

        builder.HasIndex(m => m.OccurredOnUtc)
            .HasDatabaseName("IX_Outbox_Unprocessed")
            .HasFilter("[ProcessedOnUtc] IS NULL");
    }
}