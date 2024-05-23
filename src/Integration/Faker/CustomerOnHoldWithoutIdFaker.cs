using Domain.Entities;
using Google.Cloud.Firestore;

namespace Integration.Faker;

[FirestoreData]
public class CustomerOnHoldWithoutIdFaker:CustomerOnHold
{
    public override string Id => default!;
}
