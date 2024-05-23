using Domain.Models;
using Domain.Models.CRM;
using System.Globalization;

namespace Integration.Faker;

public static class PersonFaker
{
    public static Tuple<CrmPerson?, PersonAccount?> GenerateValidPersonResponse()
    {
        var faker = new Bogus.Faker();

        var person = new CrmPerson
        {
            Created = faker.Date.Past(yearsToGoBack: 2).ToString(CultureInfo.InvariantCulture),
            CustomerUuid = null,
            DomesticAddress = new DomesticAddress
            {
                City = faker.Address.City(),
                PostalCode = faker.Address.ZipCode(),
                Province = faker.Address.State(),
                ProvinceCode = faker.Address.CountryCode(),
                ProvinceId = faker.Address.StateAbbr(),
                StreetDirection = faker.Address.StreetAddress(),
                StreetName = faker.Address.StreetName(),
                StreetSuffix = faker.Address.StreetSuffix(),
                UnitNumber = faker.Address.BuildingNumber()
            },
            Email = faker.Person.Email,
            FirstName = faker.Person.FirstName,
            InternationalAddress = null,
            LastName = faker.Person.LastName,
            MiddleName = null,
            PhoneNumbers = new List<PhoneNumber>
            {
                new()
                {
                    Number = faker.Person.Phone,
                    Type = PhoneNumberType.Personal
                }
            },
            Updated = faker.Date.Past(yearsToGoBack: 1).ToString(CultureInfo.InvariantCulture),
        };
        var personAccount = new PersonAccount
        {
            EffectiveDate = faker.Date.Past(),
            AccountNumber = faker.Random.Int().ToString(),
            AccountStatusId = faker.Random.Int(50)
        };

        return Tuple.Create<CrmPerson?, PersonAccount?>(person, personAccount);
    }

    public static Tuple<CrmPerson?, PersonAccount?> GenerateValidPersonResponseWithInternationalAddress()
    {
        var faker = new Bogus.Faker();

        var person = new CrmPerson
        {
            Created = faker.Date.Past(yearsToGoBack: 2).ToString(CultureInfo.InvariantCulture),
            CustomerUuid = null,
            DomesticAddress = null,
            Email = faker.Person.Email,
            FirstName = faker.Person.FirstName,
            InternationalAddress = new InternationalAddress
            {
                AddressLine1 = faker.Address.StreetName(),
                AddressLine2 = faker.Address.City(),
                AddressLine3 = null,
                AddressTypeId = "1",
                BpsCountryCode = null,
                CountryCode = faker.Address.CountryCode(),
                CountryId = null,
                CountryName = faker.Address.Country(),
                IsIRSTreatyCountry = null,
                IsmCountryCode = null,
                IsmResidenceCode = null,
                IsoCountryCode = null,
                PostalCode = faker.Address.ZipCode(),
                ProvinceState = faker.Address.State()
            },
            LastName = faker.Person.LastName,
            MiddleName = null,
            PhoneNumbers = new List<PhoneNumber>
            {
                new()
                {
                    Number = faker.Person.Phone,
                    Type = PhoneNumberType.Personal
                }
            },
            Updated = faker.Date.Past(yearsToGoBack: 1).ToString(CultureInfo.InvariantCulture),
        };

        var personAccount = new PersonAccount
        {
            EffectiveDate = faker.Date.Past(),
            AccountNumber = faker.Random.Int().ToString(),
            AccountStatusId = faker.Random.Int(50)
        };

        return Tuple.Create<CrmPerson?, PersonAccount?>(person, personAccount);
    }

    public static Tuple<CrmPerson?, PersonAccount?> GenerateValidPersonResponseWithNoAddress()
    {
        var faker = new Bogus.Faker();

        var person = new CrmPerson
        {
            Created = faker.Date.Past(yearsToGoBack: 2).ToString(CultureInfo.InvariantCulture),
            CustomerUuid = null,
            DomesticAddress = null,
            Email = faker.Person.Email,
            FirstName = faker.Person.FirstName,
            InternationalAddress = null,
            LastName = faker.Person.LastName,
            MiddleName = null,
            PhoneNumbers = new List<PhoneNumber>
            {
                new()
                {
                    Number = faker.Person.Phone,
                    Type = PhoneNumberType.Personal
                }
            },
            Updated = faker.Date.Past(yearsToGoBack: 1).ToString(CultureInfo.InvariantCulture),
        };

        var personAccount = new PersonAccount
        {
            EffectiveDate = faker.Date.Past(),
            AccountNumber = faker.Random.Int().ToString(),
            AccountStatusId = faker.Random.Int(50)
        };

        return Tuple.Create<CrmPerson?, PersonAccount?>(person, personAccount);
    }
}
