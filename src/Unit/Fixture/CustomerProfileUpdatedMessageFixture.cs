using Domain.Models;

namespace Unit.Fixture;

public class CustomerProfileUpdatedMessageFixture
{
    private readonly Bogus.Faker _faker = new();

    public CustomerProfileUpdatedMessage GenerateValidUpdatedCustomerMessageWithEmail()
    {
        return new CustomerProfileUpdatedMessage
        {
            Email = _faker.Person.Email,
            UserId = _faker.Random.Uuid().ToString(),
            PersonId = _faker.Random.Uuid().ToString()
        };
    }

    public CustomerProfileUpdatedMessage GenerateValidUpdatedCustomerMessageWithOnePhoneNumber()
    {
        return new CustomerProfileUpdatedMessage
        {
            PhoneNumbers = new[] { CreateCustomerPhoneNumber(_faker.Phone.PhoneNumber()) },
            UserId = _faker.Random.Uuid().ToString()
        };
    }

    public CustomerProfileUpdatedMessage GenerateValidUpdatedCustomerMessageWithTwoPhoneNumbers()
    {
        return new CustomerProfileUpdatedMessage
        {
            PhoneNumbers = new[]
            {
                CreateCustomerPhoneNumber(_faker.Phone.PhoneNumber()),
                CreateCustomerPhoneNumber(_faker.Phone.PhoneNumber()),
            },
            UserId = _faker.Random.Uuid().ToString()
        };
    }

    public CustomerProfileUpdatedMessage GenerateInvalidUpdatedCustomerMessageWithNullPhoneNumbers()
    {
        return new CustomerProfileUpdatedMessage
        {
            PhoneNumbers = null,
            UserId = _faker.Random.Uuid().ToString()
        };
    }

    public CustomerProfileUpdatedMessage GenerateInvalidUpdatedCustomerMessageWithEmptyPhoneNumbers()
    {
        return new CustomerProfileUpdatedMessage
        {
            PhoneNumbers = new List<CustomerPhoneNumber>(),
            UserId = _faker.Random.Uuid().ToString()
        };
    }

    public CustomerProfileUpdatedMessage GenerateInvalidUpdatedCustomerMessageWithWorkPhoneNumbers()
    {
        return new CustomerProfileUpdatedMessage
        {
            PhoneNumbers = new[]
            {
                CreateCustomerPhoneNumber(_faker.Phone.PhoneNumber(), PhoneNumberType.Work),
                CreateCustomerPhoneNumber(_faker.Phone.PhoneNumber(), PhoneNumberType.Work)
            },
            UserId = _faker.Random.Uuid().ToString()
        };
    }

    public CustomerProfileUpdatedMessage
        GenerateValidUpdatedCustomerMessageWithOneWorkPhoneNumberAndOnePersonalPhoneNumber()
    {
        return new CustomerProfileUpdatedMessage
        {
            PhoneNumbers = new[]
            {
                CreateCustomerPhoneNumber(_faker.Phone.PhoneNumber(), PhoneNumberType.Work),
                CreateCustomerPhoneNumber(_faker.Phone.PhoneNumber())
            },
            UserId = _faker.Random.Uuid().ToString()
        };
    }

    public CustomerProfileUpdatedMessage GenerateInvalidUpdatedCustomerMessageWithEmail()
    {
        return new CustomerProfileUpdatedMessage()
        {
            Email = _faker.Person.Email,
            UserId = _faker.Random.Uuid().ToString()
        };
    }

    public CustomerProfileUpdatedMessage GenerateInvalidUpdatedCustomerMessageWithName()
    {
        return new CustomerProfileUpdatedMessage
        {
            FirstName = _faker.Person.FirstName,
            LastName = _faker.Person.LastName,
            MiddleName = _faker.Person.FullName,
            UserId = _faker.Random.Uuid().ToString()
        };
    }

    public CustomerProfileUpdatedMessage GenerateValidUpdatedCustomerMessageWithValidDomesticAddress()
    {
        return new CustomerProfileUpdatedMessage
        {
            DomesticAddress = new CustomerDomesticAddress
            {
                DomesticAddressId = _faker.Random.Uuid().ToString(),
                StreetNumber = _faker.Address.BuildingNumber(),
                StreetName = _faker.Address.StreetName(),
                Province = _faker.Address.State(),
                City = _faker.Address.City(),
                PostalCode = _faker.Address.ZipCode("?#? #?#")
            }
        };
    }

    public CustomerProfileUpdatedMessage GenerateInvalidUpdatedCustomerMessageWithNullDomesticAddress()
    {
        return new CustomerProfileUpdatedMessage
        {
            DomesticAddress = null
        };
    }

    public CustomerProfileUpdatedMessage GenerateValidUpdatedCustomerMessageWithInvalidDomesticAddress()
    {
        return new CustomerProfileUpdatedMessage
        {
            DomesticAddress = new CustomerDomesticAddress
            {
                DomesticAddressId = _faker.Random.Uuid().ToString(),
                StreetNumber = _faker.Address.BuildingNumber(),
                StreetName = null,
                Province = _faker.Address.State(),
                City = _faker.Address.City(),
                PostalCode = _faker.Address.ZipCode("?#? #?#")
            }
        };
    }

    private static CustomerPhoneNumber CreateCustomerPhoneNumber(string phoneNumber,
        string phoneNumberType = PhoneNumberType.Personal)
    {
        return new CustomerPhoneNumber { PhoneNumber = phoneNumber, PhoneNumberType = phoneNumberType };
    }
}
