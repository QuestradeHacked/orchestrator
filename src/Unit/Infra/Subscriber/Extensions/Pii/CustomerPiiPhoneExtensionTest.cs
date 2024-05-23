using Domain.Models.Analysis;
using Domain.Models.Pii;
using Infra.Subscriber.Extensions.Pii;
using Integration.Faker;
using Xunit;

namespace Unit.Infra.Subscriber.Extensions.Pii;

public class CustomerPiiPhoneExtensionTest
{
    [Fact]
    public void GetPersonalPhoneNumber_ShouldReturnNull_WhenReceiveMessageWithNoPhone()
    {
        // Arrange
        var customerPiiUpdateMessage = CustomerPiiUpdateMessageFaker
            .GenerateUpdateMessageWithNoPersonalPhone();

        // Act
        var result = customerPiiUpdateMessage.GetPersonalPhoneNumber();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetPersonalPhoneNumber_ShouldReturnPhoneNumber_WhenReceiveUpdateMessage()
    {
        // Arrange
        var customerPiiUpdateMessage = CustomerPiiUpdateMessageFaker
            .GenerateUpdateMessage();
        var expectedPhoneNumber = customerPiiUpdateMessage!.Customer!.Profile!.Personal!
            .Phones![0];

        // Act
        var result = customerPiiUpdateMessage.GetPersonalPhoneNumber();

        // Assert
        Assert.Equivalent(expectedPhoneNumber,result);
    }

    [Fact]
    public void GetPersonalPhoneNumber_ShouldReturnPhoneNumber_WhenReceiveCreateMessage()
    {
        // Arrange
        var customerPiiUpdateMessage = CustomerPiiUpdateMessageFaker
            .GenerateCreateMessage();
        var expectedPhoneNumber = customerPiiUpdateMessage!.Customer!.Profile!.Personal!
            .Phones![0];

        // Act
        var result = customerPiiUpdateMessage.GetPersonalPhoneNumber();

        // Assert
        Assert.Equivalent(expectedPhoneNumber,result);
    }

    [Fact]
    public void HasPersonalPhoneNumber_ShouldReturnTrue_WhenReceiveCreateMessage()
    {
        // Arrange
        var customerPiiUpdateMessage = CustomerPiiUpdateMessageFaker
            .GenerateCreateMessage();

        // Act
        var result = customerPiiUpdateMessage.HasPersonalPhoneNumber();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasPersonalPhoneNumber_ShouldReturnTrue_WhenReceiveUpdateMessage()
    {
        // Arrange
        var customerPiiUpdateMessage = CustomerPiiUpdateMessageFaker
            .GenerateUpdateMessage();

        // Act
        var result = customerPiiUpdateMessage.HasPersonalPhoneNumber();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasPersonalPhoneNumber_ShouldReturnFalse_WhenReceiveUpdateWithNoPersonalPhone()
    {
        // Arrange
        var customerPiiUpdateMessage = CustomerPiiUpdateMessageFaker
            .GenerateUpdateMessageWithNoPersonalPhone();

        // Act
        var result = customerPiiUpdateMessage.HasPersonalPhoneNumber();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasPersonalPhoneNumber_ShouldReturnFalse_WhenReceiveCreateWithNoPersonalPhone()
    {
        // Arrange
        var customerPiiUpdateMessage = CustomerPiiUpdateMessageFaker
            .GenerateCreateMessageWithNoPersonalPhone();

        // Act
        var result = customerPiiUpdateMessage.HasPersonalPhoneNumber();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasPersonalPhoneChanges_ShouldReturnFalse_WhenReceiveUpdateWithNoPersonalPhone()
    {
        // Arrange
        var customerPiiUpdateMessage = CustomerPiiUpdateMessageFaker
            .GenerateUpdateMessageWithNoPersonalPhone();

        // Act
        var result = customerPiiUpdateMessage.HasPersonalPhoneChanges();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasPersonalPhoneChanges_ShouldReturnFalse_WhenReceiveCreateWithNoPersonalPhone()
    {
        // Arrange
        var customerPiiUpdateMessage = CustomerPiiUpdateMessageFaker
            .GenerateCreateMessageWithNoPersonalPhone();

        // Act
        var result = customerPiiUpdateMessage.HasPersonalPhoneChanges();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasPersonalPhoneChanges_ShouldReturnTrue_WhenReceiveUpdateMessage()
    {
        // Arrange
        var customerPiiUpdateMessage = CustomerPiiUpdateMessageFaker
            .GenerateUpdateMessage();

        // Act
        var result = customerPiiUpdateMessage.HasPersonalPhoneChanges();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasPersonalPhoneChanges_ShouldReturnTrue_WhenReceiveCreateMessage()
    {
        // Arrange
        var customerPiiUpdateMessage = CustomerPiiUpdateMessageFaker
            .GenerateCreateMessage();

        // Act
        var result = customerPiiUpdateMessage.HasPersonalPhoneChanges();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void PhoneUpdateHandler_ShouldReturnList_WhenReceivePhoneUpdateWithNoAddressChanges()
    {
        // Arrange
        var customerPiiUpdateMessage = CustomerPiiUpdateMessageFaker.GenerateUpdateMessageWithNoAddressChanges();

        // Act
        var result = customerPiiUpdateMessage.PhoneUpdateHandler();

        // Assert
        Assert.Equal(2,result!.Count);
        Assert.True
        (
            result.Exists
            (
                r => r is CustomerPhoneAnalysisRequest
            )
        );

        Assert.True
        (
            result.Exists
            (
                r => r is CustomerIdentityAnalysisRequest
            )
        );
    }

    [Fact]
    public void PhoneUpdateHandler_ShouldReturnNull_WhenReceiveNoPhoneChanges()
    {
        // Arrange
        var customerPiiUpdateMessage = CustomerPiiUpdateMessageFaker.GenerateBasicUpdateMessage();

        // Act
        var result = customerPiiUpdateMessage.PhoneUpdateHandler();

        // Assert
        Assert.Null(result);
    }
}
