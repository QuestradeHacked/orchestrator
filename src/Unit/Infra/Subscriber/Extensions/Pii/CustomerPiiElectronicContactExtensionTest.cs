using Domain.Models.Pii;
using Infra.Subscriber.Extensions.Pii;
using Integration.Faker;
using Xunit;

namespace Unit.Infra.Subscriber.Extensions.Pii;

public class CustomerPiiElectronicContactExtensionTest
{

    [Fact]
    public void HasPrimaryEmail_ShouldReturnTrue_WhenReceiveUpdateMessage()
    {
        // Arrange
        var customerPiiUpdateMessage = CustomerPiiUpdateMessageFaker.GenerateUpdateMessage();

        // Act
        var result = customerPiiUpdateMessage.HasPrimaryEmail();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasPrimaryEmail_ShouldReturnFalse_WhenReceiveUpdateMessageWithNoEmail()
    {
        // Arrange
        var customerPiiUpdateMessage = CustomerPiiUpdateMessageFaker.GenerateUpdateMessage();
        customerPiiUpdateMessage.Customer!.Profile!.Personal!.ElectronicContacts =
            new List<CustomerPiiElectronicContact>();
        customerPiiUpdateMessage.DeltaChanges!.Remove
        (
            customerPiiUpdateMessage.DeltaChanges!.Where
            (
                e => e.Path!.ToLower() == "/profile/personal/electroniccontacts"
            ).First()
        );
        // Act
        var result = customerPiiUpdateMessage.HasPrimaryEmail();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasPrimaryEmail_ShouldReturnTrue_WhenReceiveCreateMessage()
    {
        // Arrange
        var customerPiiUpdateMessage = CustomerPiiUpdateMessageFaker.GenerateCreateMessage();

        // Act
        var result = customerPiiUpdateMessage.HasPrimaryEmail();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasPrimaryEmail_ShouldReturnFalse_WhenReceiveCreateMessageWithNoEmail()
    {
        // Arrange
        var customerPiiUpdateMessage = CustomerPiiUpdateMessageFaker.GenerateUpdateMessage();
        customerPiiUpdateMessage.Customer!.Profile!.Personal!.ElectronicContacts =
            new List<CustomerPiiElectronicContact>();

        // Act
        var result = customerPiiUpdateMessage.HasPrimaryEmail();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void EmailUpdateHandler_ShouldReturnList_WhenReceiveEmailUpdateWithNoAddressChanges()
    {
        // Arrange
        var customerPiiUpdateMessage = CustomerPiiUpdateMessageFaker.GenerateUpdateMessageWithNoAddressChanges();
        var expectedEmail = customerPiiUpdateMessage.Customer!.Profile!.Personal!
            .ElectronicContacts![0].Value;

        // Act
        var result = customerPiiUpdateMessage.EmailUpdateHandler();

        // Assert
        Assert.Equal(result!.Email, expectedEmail );
    }

    [Fact]
    public void EmailUpdateHandler_ShouldReturnNull_WhenReceiveNoEmailChanges()
    {
        // Arrange
        var customerPiiUpdateMessage = CustomerPiiUpdateMessageFaker.GenerateUpdateMessage();
        customerPiiUpdateMessage.Customer!.Profile!.Personal!.ElectronicContacts = new List<CustomerPiiElectronicContact>();
        customerPiiUpdateMessage.DeltaChanges.Remove(
            customerPiiUpdateMessage.DeltaChanges.Where( d =>
                d.Path == "/Profile/Personal/ElectronicContacts").First()
        );

        // Act
        var result = customerPiiUpdateMessage.EmailUpdateHandler();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetPrimaryEmail_ShouldReturnEmail_WhenReceiveMessageWithPrimaryEmail()
    {
        // Arrange
        var customerPiiUpdateMessage = CustomerPiiUpdateMessageFaker.GenerateCreateMessageWithNoPersonalPhone();
        var expectedEmail = customerPiiUpdateMessage!.Customer!.Profile!.Personal!
            .ElectronicContacts.First();
        // Act
        var result = customerPiiUpdateMessage.GetPrimaryEmail();

        // Assert
        Assert.Equivalent(expectedEmail,result);
    }

    [Fact]
    public void GetPrimaryEmail_ShouldReturnNull_WhenReceiveMessageWithNoPrimaryEmail()
    {
        // Arrange
        var customerPiiUpdateMessage = CustomerPiiUpdateMessageFaker.GenerateCreateMessageWithNoPersonalPhone();
        customerPiiUpdateMessage!.Customer!.Profile!.Personal!
            .ElectronicContacts = new List<CustomerPiiElectronicContact>();
        // Act
        var result = customerPiiUpdateMessage.GetPrimaryEmail();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void HasEmailChanges_ShouldReturnTrue_WhenReceiveCreateEmailMessage()
    {
        // Arrange
        var customerPiiUpdateMessage = CustomerPiiUpdateMessageFaker.GenerateCreateMessage();

        // Act
        var result = customerPiiUpdateMessage.HasPrimaryEmailChanges();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasEmailChanges_ShouldReturnTrue_WhenReceiveUpdateEmailMessage()
    {
        // Arrange
        var customerPiiUpdateMessage = CustomerPiiUpdateMessageFaker.GenerateUpdateMessage();

        // Act
        var result = customerPiiUpdateMessage.HasPrimaryEmailChanges();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasEmailChanges_ShouldReturnFalse_WhenReceiveCreateWithNoPrimaryEmail()
    {
        // Arrange
        var customerPiiUpdateMessage = CustomerPiiUpdateMessageFaker.GenerateCreateMessage();
        customerPiiUpdateMessage.Customer!.Profile!.Personal!.ElectronicContacts[0].Type="test";

        // Act
        var result = customerPiiUpdateMessage.HasPrimaryEmailChanges();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasEmailChanges_ShouldReturnFalse_WhenReceiveUpdateWithNoPrimaryEmailActivate()
    {
        // Arrange
        var customerPiiUpdateMessage = CustomerPiiUpdateMessageFaker.GenerateUpdateMessage();
        customerPiiUpdateMessage.Customer!.Profile!.Personal!.ElectronicContacts[0].Status="test";

        // Act
        var result = customerPiiUpdateMessage.HasPrimaryEmailChanges();

        // Assert
        Assert.False(result);
    }
}
