using Domain.Exceptions;
using Domain.Models.Analysis;
using Infra.Subscriber.Extensions.Pii;
using Integration.Faker;
using Xunit;

namespace Unit.Infra.Subscriber.Extensions.Pii;

public class CustomerPiiUpdateMessageExtensionTest
{
    [Fact]
    public void ToCustomerPhoneAnalysisRequest_ShouldReturnAnalysisRequest_WhenReceiveCreateMessage()
    {
       // Arrange
        var customerPiiUpdateMessage = CustomerPiiUpdateMessageFaker.GenerateCreateMessage();
        var data = customerPiiUpdateMessage;
        var phoneNumbers = new List<string>
        {
            customerPiiUpdateMessage.GetPersonalPhoneNumber()!.PhoneNumber!
        };
        var expectedCustomerPhoneAnalysisRequest = new CustomerPhoneAnalysisRequest{
                CrmUserId = customerPiiUpdateMessage.GetCrmUserId(),
                EnterpriseProfileId = data.Customer!.MasterProfileId,
                PhoneNumbers = phoneNumbers,
                ProfileId = data.Customer!.Id};


       // Act
       var result = customerPiiUpdateMessage.ToCustomerPhoneAnalysisRequest();

       // Assert
       Assert.Equivalent(expectedCustomerPhoneAnalysisRequest, result);
    }

    [Fact]
    public void ToCustomerPhoneAnalysisRequest_ShouldReturnException_WhenConversionFail()
    {
        // Arrange
        var customerPiiUpdateMessage = CustomerPiiUpdateMessageFaker.GenerateCreateMessage();
        customerPiiUpdateMessage.Customer = null;

        // Act
        var result = Record.Exception(() =>
            customerPiiUpdateMessage.ToCustomerPhoneAnalysisRequest());

        // Assert
        Assert.IsType<ConvertToAnalysisRequestException>(result);
        Assert.Contains("Error converting to AnalysisRequest."
            ,result.Message);
    }

    [Fact]
    public void ToCustomerEmailAnalysisRequest_ShouldReturnAnalysisRequest_WhenReceiveCreateMessage()
    {
       // Arrange
        var customerPiiUpdateMessage = CustomerPiiUpdateMessageFaker.GenerateCreateMessage();
        var data = customerPiiUpdateMessage;
        var electronicContact = customerPiiUpdateMessage.GetPrimaryEmail()!;
        var primaryAddress = customerPiiUpdateMessage.GetPrimaryAddress()!;

        var expectedCustomerEmailAnalysisRequest = new CustomerEmailAnalysisRequest
        {
            City = primaryAddress.City!,
            Country = primaryAddress.Country!,
            CrmUserId = customerPiiUpdateMessage.GetCrmUserId(),
            Email = electronicContact.Value!,
            EnterpriseProfileId = data.Customer!.MasterProfileId,
            FirstName = data.Customer!.Profile!.Personal!.FirstName!,
            LastName = data.Customer!.Profile!.Personal!.LastName!,
            PhoneNumber = customerPiiUpdateMessage.GetPersonalPhoneNumber()!.PhoneNumber!,
            ProfileId = data.Customer!.Id,
            State = primaryAddress.Province!,
            Street = primaryAddress.StreetName!,
            UpdatedAt = electronicContact.UpdatedAt,
            ZipCode = primaryAddress.PostalCode!
        };


       // Act
       var result = customerPiiUpdateMessage.ToCustomerEmailAnalysisRequest();

       // Assert
       Assert.Equivalent(expectedCustomerEmailAnalysisRequest, result);
    }

    [Fact]
    public void ToCustomerEmailAnalysisRequest_ShouldReturnException_WhenConversionFail()
    {
        // Arrange
        var customerPiiUpdateMessage = CustomerPiiUpdateMessageFaker.GenerateCreateMessage();
        customerPiiUpdateMessage.Customer = null;

        // Act
        var result = Record.Exception(() =>
            customerPiiUpdateMessage.ToCustomerEmailAnalysisRequest());

        // Assert
        Assert.IsType<ConvertToAnalysisRequestException>(result);
        Assert.Contains("Error converting to AnalysisRequest.",result.Message);
    }

    [Fact]
    public void ToCustomerIdentityAnalysisRequest_ShouldReturnAnalysisRequest_WhenReceiveCreateMessage()
    {
       // Arrange
        var customerPiiUpdateMessage = CustomerPiiUpdateMessageFaker.GenerateCreateMessage();

        var address = customerPiiUpdateMessage.GetPrimaryAddress()!;
        var data = customerPiiUpdateMessage;
        var expectedCustomerPhoneAnalysisRequest = new CustomerIdentityAnalysisRequest
        {
            AddressLine1 = $"{address.StreetNumber} {address.StreetName}",
            AddressCountryCode =address.CountryCode,
            City = address.City,
            CrmUserId = customerPiiUpdateMessage.GetCrmUserId(),
            DateOfBirth = data.Customer!.Profile!.Personal!.DateOfBirth,
            EnterpriseProfileId = data.Customer!.MasterProfileId,
            FirstName = data.Customer.Profile.Personal.FirstName,
            LastName = data.Customer.Profile.Personal.LastName,
            NationalId = null,
            PhoneNumbers = new List<string>
            {
                customerPiiUpdateMessage.GetPersonalPhoneNumber()!.PhoneNumber!
            },
            PostalCode = address.PostalCode,
            ProfileId = data.Customer!.Id,
            State = address.Province
        };

       // Act
       var result = customerPiiUpdateMessage.ToCustomerIdentityAnalysisRequest();

       // Assert
       Assert.Equivalent(expectedCustomerPhoneAnalysisRequest, result);
    }

    [Fact]
    public void ToCustomerIdentityAnalysisRequest_ShouldReturnException_WhenConversionFail()
    {
        // Arrange
        var customerPiiUpdateMessage = CustomerPiiUpdateMessageFaker.GenerateCreateMessage();
        customerPiiUpdateMessage.Customer = null;


        // Act
        var result = Record.Exception(() =>
            customerPiiUpdateMessage.ToCustomerIdentityAnalysisRequest());

        // Assert
        Assert.IsType<ConvertToAnalysisRequestException>(result);
        Assert.Contains("Error converting to AnalysisRequest.",result.Message);
    }
}
