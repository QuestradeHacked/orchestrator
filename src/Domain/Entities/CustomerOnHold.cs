using Google.Cloud.Firestore;

namespace Domain.Entities;

[FirestoreData]
public class CustomerOnHold : BaseEntity
{
    public override string Id => $"{PersonId}_{UserId}";

    [FirestoreProperty] public string PersonId { get; set; } = default!;

    [FirestoreProperty] public string UserId { get; set; } = default!;
}
