using Domain.Entities.Converters;
using Google.Cloud.Firestore;

namespace Domain.Entities;

public abstract class BaseEntity
{
    [FirestoreDocumentId]
    public virtual string Id { get; private set; } = default!;

    [FirestoreProperty(ConverterType = typeof(DateTimeConverter))]
    public virtual DateTime CreatedAt { get; set; }
}
