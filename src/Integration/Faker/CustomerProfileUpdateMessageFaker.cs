using Domain.Models;

namespace Integration.Faker;

public static class CustomerProfileUpdateMessageFaker
{
    public static PubSubMessage<CustomerProfileUpdatedMessage> GenerateValidCustomerProfileUpdateMessage()
    {
        var faker = new Bogus.Faker();

        var generatedValidCustomerProfileUpdateMessage = new PubSubMessage<CustomerProfileUpdatedMessage>
        {
            Id = faker.Random.Uuid().ToString(),
            Data = new CustomerProfileUpdatedMessage
            {
                PhoneNumbers = new List<CustomerPhoneNumber>
                {
                    new()
                    {
                        PhoneNumber = faker.Phone.PhoneNumber(),
                        PhoneNumberType = PhoneNumberType.Personal
                    }
                },
                UserId = faker.Random.Int(0).ToString()
            }
        };

        return generatedValidCustomerProfileUpdateMessage;
    }

    public static PubSubMessage<CustomerProfileUpdatedMessage> GenerateValidCustomerProfileUpdateMessageWithEmail()
    {
        var faker = new Bogus.Faker();

        var generatedValidCustomerProfileUpdateMessage = new PubSubMessage<CustomerProfileUpdatedMessage>
        {
            Id = faker.Random.Uuid().ToString(),
            Data = new CustomerProfileUpdatedMessage
            {
                Email = faker.Person.Email,
                PhoneNumbers = new List<CustomerPhoneNumber>
                {
                    new()
                    {
                        PhoneNumber = faker.Phone.PhoneNumber(),
                        PhoneNumberType = PhoneNumberType.Personal
                    }
                },
                UserId = faker.Random.Int(0).ToString()
            }
        };

        return generatedValidCustomerProfileUpdateMessage;
    }

    public static PubSubMessage<CustomerProfileUpdatedMessage>
        GenerateValidCustomerProfileUpdateMessageWithEmailAndPhoneNumber()
    {
        var faker = new Bogus.Faker();

        var generatedValidCustomerProfileUpdateMessage = new PubSubMessage<CustomerProfileUpdatedMessage>
        {
            Id = faker.Random.Uuid().ToString(),
            Data = new CustomerProfileUpdatedMessage
            {
                Email = faker.Person.Email,
                PhoneNumbers = new List<CustomerPhoneNumber>
                {
                    new()
                    {
                        PhoneNumber = faker.Phone.PhoneNumber(),
                        PhoneNumberType = PhoneNumberType.Personal
                    }
                },
                UserId = faker.Random.Int(0).ToString()
            }
        };

        return generatedValidCustomerProfileUpdateMessage;
    }

    public static PubSubMessage<CustomerProfileUpdatedMessage>
        GenerateCustomerProfileUpdateMessageWithPersonalAndWorkPhoneNumbers()
    {
        var faker = new Bogus.Faker();

        var generatedValidCustomerProfileUpdateMessage = new PubSubMessage<CustomerProfileUpdatedMessage>
        {
            Id = faker.Random.Uuid().ToString(),
            Data = new CustomerProfileUpdatedMessage
            {
                PhoneNumbers = new List<CustomerPhoneNumber>
                {
                    new()
                    {
                        PhoneNumber = faker.Phone.PhoneNumber(),
                        PhoneNumberType = PhoneNumberType.Work
                    },
                    new()
                    {
                        PhoneNumber = faker.Phone.PhoneNumber(),
                        PhoneNumberType = PhoneNumberType.Personal
                    }
                },
                UserId = faker.Random.Int(0).ToString()
            }
        };

        return generatedValidCustomerProfileUpdateMessage;
    }

    public static PubSubMessage<CustomerProfileUpdatedMessage> GenerateInvalidWorkPhoneCustomerProfileUpdateMessage()
    {
        var faker = new Bogus.Faker();

        var generatedValidCustomerProfileUpdateMessage = new PubSubMessage<CustomerProfileUpdatedMessage>
        {
            Id = faker.Random.Uuid().ToString(),
            Data = new CustomerProfileUpdatedMessage
            {
                PhoneNumbers = new List<CustomerPhoneNumber>
                {
                    new()
                    {
                        PhoneNumber = faker.Phone.PhoneNumber(),
                        PhoneNumberType = PhoneNumberType.Work
                    }
                },
                UserId = faker.Random.Int(0).ToString()
            }
        };

        return generatedValidCustomerProfileUpdateMessage;
    }

    public static PubSubMessage<CustomerProfileUpdatedMessage>
        GenerateValidCustomerProfileUpdateMessageWithValidDomesticAddress()
    {
        var faker = new Bogus.Faker();

        var generatedValidCustomerProfileUpdateMessage = new PubSubMessage<CustomerProfileUpdatedMessage>
        {
            Id = faker.Random.Uuid().ToString(),
            Data = new CustomerProfileUpdatedMessage
            {
                DomesticAddress = new CustomerDomesticAddress()
                {
                    DomesticAddressId = faker.Random.Uuid().ToString(),
                    StreetNumber = faker.Address.BuildingNumber(),
                    StreetName = faker.Address.StreetName(),
                    Province = faker.Address.State(),
                    City = faker.Address.City(),
                    PostalCode = faker.Address.ZipCode("?#? #?#"),
                },
                UserId = faker.Random.Int(0).ToString()
            }
        };

        return generatedValidCustomerProfileUpdateMessage;
    }

    public static PubSubMessage<CustomerProfileUpdatedMessage>
        GenerateCustomerProfileUpdateMessageWithNoRelevantInformation()
    {
        var faker = new Bogus.Faker();

        var CustomerProfileUpdateMessageWithNoHandler = new PubSubMessage<CustomerProfileUpdatedMessage>
        {
            Id = faker.Random.Uuid().ToString(),
            Data = new CustomerProfileUpdatedMessage
            {
                UserId = faker.Random.Int(0).ToString()
            }
        };

        return CustomerProfileUpdateMessageWithNoHandler;
    }
}
