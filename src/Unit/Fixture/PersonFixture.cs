using Domain.Models;
using Domain.Models.CRM;
using System.Globalization;

namespace Unit.Fixture;

public class PersonFixture
{
    private readonly Bogus.Faker _faker = new();

    public CrmPerson GenerateValidPerson()
    {
        return new CrmPerson
        {
            Created = _faker.Date.Past(yearsToGoBack: 2).ToString(CultureInfo.InvariantCulture),
            FirstName = _faker.Person.FirstName,
            LastName = _faker.Person.LastName,
            PhoneNumbers = new List<PhoneNumber>
            {
                new() { Number = _faker.Person.Phone, Type = PhoneNumberType.Personal }
            },
            Updated = _faker.Date.Past().ToString(CultureInfo.InvariantCulture),
            DomesticAddress = new DomesticAddress
            {
                City = _faker.Person.Address.City,
                Province = _faker.Person.Address.State,
                StreetName = _faker.Person.Address.Street,
                PostalCode = _faker.Person.Address.ZipCode,
            }
        };
    }

    public CrmPerson GenerateInvalidPersonWithoutPersonalNumber()
    {
        return new CrmPerson
        {
            Created = _faker.Date.Past(yearsToGoBack: 2).ToString(CultureInfo.InvariantCulture),
            FirstName = _faker.Person.FirstName,
            LastName = _faker.Person.LastName,
            PhoneNumbers = new List<PhoneNumber>
            {
                new() { Number = _faker.Person.Phone, Type = PhoneNumberType.Work }
            },
            Updated = _faker.Date.Past().ToString(CultureInfo.InvariantCulture),
            DomesticAddress = new DomesticAddress
            {
                City = _faker.Person.Address.City,
                Province = _faker.Person.Address.State,
                StreetName = _faker.Person.Address.Street,
                PostalCode = _faker.Person.Address.ZipCode,
            }
        };
    }

    public CrmPerson GenerateInvalidPersonWithoutDomesticAddress()
    {
        return new CrmPerson
        {
            Created = _faker.Date.Past(yearsToGoBack: 2).ToString(CultureInfo.InvariantCulture),
            FirstName = _faker.Person.FirstName,
            LastName = _faker.Person.LastName,
            PhoneNumbers = new List<PhoneNumber>
            {
                new() { Number = _faker.Person.Phone, Type = PhoneNumberType.Personal }
            },
            Updated = _faker.Date.Past().ToString(CultureInfo.InvariantCulture),
        };
    }
}
