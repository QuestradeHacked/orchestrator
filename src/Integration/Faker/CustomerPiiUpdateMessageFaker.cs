using Domain.Constants;
using Domain.Models;
using Domain.Models.Pii;


namespace Integration.Faker;


public static class CustomerPiiUpdateMessageFaker
{
    private static readonly Bogus.Faker _faker = new Bogus.Faker();

    public static PubSubMessage<CustomerPiiUpdateMessage> GeneratePubSubMessage(CustomerPiiUpdateMessage customerPiiUpdateMessage)
    {
        var generatedCustomerPiiUpdateMessage = new PubSubMessage<CustomerPiiUpdateMessage>
        {
            Id = Guid.NewGuid().ToString(),
            Data =customerPiiUpdateMessage
        };

        return generatedCustomerPiiUpdateMessage;
    }

    public static CustomerPiiUpdateMessage GenerateUpdateMessageWithNoAddressChanges()
    {
        var generatedCustomerPiiUpdateMessage = GenerateUpdateMessage();
        generatedCustomerPiiUpdateMessage.DeltaChanges.Remove
        (
            generatedCustomerPiiUpdateMessage.DeltaChanges.Where
            (
                d => d.Path == "/Profile/Personal/Addresses"
            ).First()
        );

        return generatedCustomerPiiUpdateMessage;
    }

    public static CustomerPiiUpdateMessage GenerateEmailUpdateMessage()
    {
        var generatedCustomerPiiUpdateMessage = GenerateUpdateMessage();
        generatedCustomerPiiUpdateMessage.DeltaChanges.Remove
        (
            generatedCustomerPiiUpdateMessage.DeltaChanges.Where
            (
                d => d.Path == "/Profile/Personal/Addresses"
            ).First()
        );

        generatedCustomerPiiUpdateMessage.DeltaChanges.Remove
        (
            generatedCustomerPiiUpdateMessage.DeltaChanges.Where
            (
                d => d.Path == "/Profile/Personal/Phones"
            ).First()
        );

        return generatedCustomerPiiUpdateMessage;
    }

    public static CustomerPiiUpdateMessage GeneratePhoneUpdateMessage()
    {
        var generatedCustomerPiiUpdateMessage = GenerateUpdateMessage();
        generatedCustomerPiiUpdateMessage.DeltaChanges.Remove
        (
            generatedCustomerPiiUpdateMessage.DeltaChanges.Where
            (
                d => d.Path == "/Profile/Personal/Addresses"
            ).First()
        );

        generatedCustomerPiiUpdateMessage.DeltaChanges.Remove
        (
            generatedCustomerPiiUpdateMessage.DeltaChanges.Where
            (
                d => d.Path == "/Profile/Personal/ElectronicContacts"
            ).First()
        );

        return generatedCustomerPiiUpdateMessage;
    }


    public static CustomerPiiUpdateMessage GenerateUpdateMessageWithNoAddress()
    {
        var generatedCustomerPiiUpdateMessage = GenerateUpdateMessage();

        generatedCustomerPiiUpdateMessage.Customer!.Profile!.Personal!.Addresses
            = new List<CustomerPiiAddress>();

        generatedCustomerPiiUpdateMessage.DeltaChanges.Remove
        (
            generatedCustomerPiiUpdateMessage.DeltaChanges.Where
            (
                d => d.Path == "/Profile/Personal/Addresses"
            ).First()
        );


        return generatedCustomerPiiUpdateMessage;
    }

    public static CustomerPiiUpdateMessage GenerateUpdateMessageWithNoPersonalPhone()
    {
        var generatedCustomerPiiUpdateMessage = GenerateCreateMessage();
        generatedCustomerPiiUpdateMessage.Customer!.Profile!.Personal!.Phones
            = new List<CustomerPiiPhone>();

        return generatedCustomerPiiUpdateMessage;
    }

    public static CustomerPiiUpdateMessage GenerateCreateMessageWithNoAddress()
    {
        var generatedCustomerPiiUpdateMessage = GenerateCreateMessage();
        generatedCustomerPiiUpdateMessage.Customer!.Profile!.Personal!.Addresses =
            new List<CustomerPiiAddress>();

        return generatedCustomerPiiUpdateMessage;
    }

     public static CustomerPiiUpdateMessage GenerateCreateMessageWithNoPersonalPhone()
    {
        var generatedCustomerPiiUpdateMessage = GenerateCreateMessage();
        generatedCustomerPiiUpdateMessage.Customer!.Profile!.Personal!.Phones =
            new  List<CustomerPiiPhone>();

        return generatedCustomerPiiUpdateMessage;
    }

    public static CustomerPiiUpdateMessage GenerateCreateMessage()
    {
        var Address = GetBasicCustomerPiiAddress();
        var customerPiiUpdateMessage = GenerateBasicUpdateMessage();
        var electronicContact = GetBasicCustomerPiiElectronicContact();
        var phone = GetBasicCustomerPiiPhone();

        customerPiiUpdateMessage.Customer!.Profile!.Personal!.Addresses = new List<CustomerPiiAddress>{ Address };
        customerPiiUpdateMessage.Customer.Profile.Personal.ElectronicContacts = new List<CustomerPiiElectronicContact>{ electronicContact };
        customerPiiUpdateMessage.Customer.Profile.Personal.Phones = new List<CustomerPiiPhone>{ phone };

        return customerPiiUpdateMessage;
    }

    public static CustomerPiiUpdateMessage GenerateUpdateMessage()
    {
        var address = GetBasicCustomerPiiAddress();
        var customerPiiUpdateMessage = GenerateBasicUpdateMessage();
        var deltaChanges = new List<CustomerPiiDeltaChange>();
        var electronicContact = GetBasicCustomerPiiElectronicContact();
        var phone = GetBasicCustomerPiiPhone();

        deltaChanges.Add
        (
            GetBasicCustomerPiiDeltaChange
            (
                "add",
                 "/Profile/Personal/Addresses",
                new List<CustomerPiiAddress>{ address }
            )
        );

        deltaChanges.Add
        (
            GetBasicCustomerPiiDeltaChange
            (
                "add",
                "/Profile/Personal/ElectronicContacts",
                new List<CustomerPiiElectronicContact>{ electronicContact }
            )
        );

        deltaChanges.Add
        (
            GetBasicCustomerPiiDeltaChange
            (
                "add",
                "/Profile/Personal/Phones",
                new List<CustomerPiiPhone>{phone}
            )
        );

        customerPiiUpdateMessage.Customer!.Profile!.Personal!.Addresses = new List<CustomerPiiAddress>{ address };
        customerPiiUpdateMessage.DeltaChanges = deltaChanges;
        customerPiiUpdateMessage.Customer.Profile.Personal.ElectronicContacts = new List<CustomerPiiElectronicContact>{ electronicContact };
        customerPiiUpdateMessage.Customer.Profile.Personal.Phones = new List<CustomerPiiPhone>{ phone };

        return customerPiiUpdateMessage;
    }

    public static CustomerPiiUpdateMessage GenerateBasicUpdateMessage()
    {
        var customer = GetBasicCustomerPii();
        customer.Profile = new CustomerPiiProfile
        {
            Personal = new()
        };

        customer.RelatedReferences = GetBasicCustomerPiiRelatedReferences();

        return new CustomerPiiUpdateMessage
        {
            DeltaChanges = new(),
            Customer = customer
        };

    }

    private static CustomerPiiElectronicContact GetBasicCustomerPiiElectronicContact()
    {
        return new CustomerPiiElectronicContact
        {
            Id = $"primaryemail-{Guid.NewGuid()}",
            Type = "Email",
            Subtype = "Email",
            Value = _faker.Internet.Email(),
            Status = "Active",
            UpdatedAt = DateTime.UtcNow.ToString(),
            IsVerified = true
        };
    }

    private static CustomerPiiAddress GetBasicCustomerPiiAddress()
    {
        var city = _faker.Address.City();
        var streetName = _faker.Address.StreetName();
        var province =  _faker.Address.State();
        var postalCode = _faker.Address.ZipCode();

        return new CustomerPiiAddress
        {
            Id = $"primary-{Guid.NewGuid()}",
            FormattedAddress =
            {
               streetName,
               city,
               $"{province} {postalCode}"
            },
            PostalCode = postalCode,
            Country = _faker.Address.County(),
            CountryCode = _faker.Address.CountryCode(),
            Type = "Primary",
            Province =province,
            City = city,
            StreetName = streetName,
            Status = "Active",
            UpdatedAt = DateTime.UtcNow.ToString()
        };
    }

    private static CustomerPiiCustomer GetBasicCustomerPii()
    {
        return new CustomerPiiCustomer
        {
            MasterProfileId =  Guid.NewGuid().ToString(),
            Revision =  _faker.Date.Past().ToUniversalTime().ToString(),
            IsIdentityVerified =  true,
            IsQuestradeEmployee =  false,
            IsDeactivated =  false,
            Id =  Guid.NewGuid().ToString()
        };
    }

    private static List<CustomerPiiRelatedReference> GetBasicCustomerPiiRelatedReferences()
    {
        return new List<CustomerPiiRelatedReference>()
        {
            new CustomerPiiRelatedReference
            {
                Id = _faker.Random.Int(11111111,99999999).ToString(),
                Name = "CRM",
                AttributeName = "PersonID"
            },
            new CustomerPiiRelatedReference
            {
                Id = Guid.NewGuid().ToString(),
                Name = "CRM",
                AttributeName = "CustomerUUID"
            },
            new CustomerPiiRelatedReference
            {
                Id = _faker.Random.Int(1111111,9999999).ToString(),
                Name = "CRM",
                AttributeName = "UserId"
            }
        };
    }

    private static CustomerPiiDeltaChange GetBasicCustomerPiiDeltaChange(string operation, string path, object value)
    {
        return new CustomerPiiDeltaChange
        {
            Operation = operation,
            Path = path,
            Value = value,
        };
    }

    private static CustomerPiiPhone GetBasicCustomerPiiPhone()
    {
        var areaCode = _faker.Random.Int(111,999).ToString();
        var countryCode = _faker.Random.Int(1,9).ToString();
        var exchange = _faker.Random.Int(111,999).ToString();
        var localNumber = _faker.Random.Int(1111,9999).ToString();

        return new CustomerPiiPhone
        {
            AreaCode = areaCode,
            CountryCode = countryCode,
            Exchange = exchange,
            Id = $"{CustomerPiiPhoneNumberType.Personal}-{Guid.NewGuid()}",
            IsVerified = true,
            LocalNumber = localNumber,
            PhoneNumber = $"+{countryCode}{areaCode}{exchange}{localNumber}",
            PhoneNumberType = CustomerPiiPhoneNumberType.Personal ,
            Status = "Active",
            UpdatedAt = DateTime.UtcNow.ToString()
        };
    }
}
