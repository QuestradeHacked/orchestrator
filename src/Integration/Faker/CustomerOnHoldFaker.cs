using Domain.Entities;

namespace Integration.Faker;

public static class CustomerOnHoldFaker
{
    public static CustomerOnHold GenerateValidCustomerOnHold()
    {
        var faker = new Bogus.Faker();
        return new CustomerOnHold
        {
            CreatedAt = DateTime.Now.ToUniversalTime(),
            PersonId = faker.Random.Int(1000,9999).ToString(),
            UserId = faker.Random.Int(1000,9999).ToString()
        };
    }

    public static CustomerOnHold GenerateNoIdCustomerOnHold()
    {
        var faker = new Bogus.Faker();
        return new CustomerOnHoldWithoutIdFaker
        {
            CreatedAt = DateTime.Now.ToUniversalTime(),
            PersonId = faker.Random.Int(1000,9999).ToString(),
            UserId = faker.Random.Int(1000,9999).ToString()
        };
    }
}
