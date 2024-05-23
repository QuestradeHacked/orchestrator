using Infra.Subscriber.Extensions.Pii;
using Integration.Faker;
using Xunit;

namespace Unit.Infra.Subscriber.Extensions.Pii;

public class CustomerPiiAddressExtensionTest
{
    [Fact]
    public void HasPrimaryAddressCreated_ShouldReturnFalse_WhenReceivesPrimaryAddressUpdate()
    {
        // Arrange
        var customerUpdatePiiMessage = CustomerPiiUpdateMessageFaker.GenerateUpdateMessage();

        // Act
        var result = customerUpdatePiiMessage.HasPrimaryAddressCreated();

        //Assert
        Assert.False(result);
    }

    [Fact]
    public void HasPrimaryAddressCreated_ShouldReturnTrue_WhenReceivesNewPrimaryAddress()
    {
        // Arrange
        var customerUpdatePiiMessage = CustomerPiiUpdateMessageFaker.GenerateCreateMessage();

        // Act
        var result = customerUpdatePiiMessage.HasPrimaryAddressCreated();

        //Assert
        Assert.True(result);
    }

    [Fact]
    public void HasPrimaryAddressChanges_ShouldReturnFalse_WhenReceivesNoPrimaryAddressUpdated()
    {
        // Arrange
        var customerPiiUpdateMessage = CustomerPiiUpdateMessageFaker
            .GenerateUpdateMessageWithNoAddressChanges();

        // Act
        var result = customerPiiUpdateMessage.HasPrimaryAddressCreated();

        //Assert
        Assert.False(result);
    }

    [Fact]
    public void HasPrimaryAddressCreated_ShouldReturnFalse_WhenReceivesNoPrimaryAddressCreated()
    {
        // Arrange
        var customerPiiUpdateMessage = CustomerPiiUpdateMessageFaker
            .GenerateCreateMessageWithNoAddress();

        // Act
        var result = customerPiiUpdateMessage.HasPrimaryAddressCreated();

        //Assert
        Assert.False(result);
    }

    [Fact]
    public void GetPrimaryAddress_ShouldReturnAddress_WhenReceivesMessageWithAddress()
    {
        // Arrange
        var customerUpdatePiiMessage = CustomerPiiUpdateMessageFaker.GenerateCreateMessage();
        var expectedAddress = customerUpdatePiiMessage.Customer!.Profile!.Personal!
            .Addresses!.First();
        // Act
        var result = customerUpdatePiiMessage.GetPrimaryAddress();

        //Assert
        Assert.Equivalent(result,expectedAddress);
    }

    [Fact]
    public void GetPrimaryAddress_ShouldReturnNull_WhenReceivesMessageWithNoAddress()
    {
        // Arrange
        var customerUpdatePiiMessage = CustomerPiiUpdateMessageFaker
            .GenerateCreateMessageWithNoAddress();

        // Act
        var result = customerUpdatePiiMessage.GetPrimaryAddress();

        //Assert
        Assert.Null(result);
    }

    [Fact]
    public void HasPrimaryAddress_ShouldReturnTrue_WhenReceivesMessageWithAddress()
    {
        // Arrange
        var customerUpdatePiiMessage = CustomerPiiUpdateMessageFaker.GenerateUpdateMessage();

        // Act
        var result = customerUpdatePiiMessage.HasPrimaryAddress();

        //Assert
        Assert.True(result);
    }

    [Fact]
    public void HasPrimaryAddress_ShouldReturnFalse_WhenReceivesMessageWithNoAddress()
    {
        // Arrange
        var customerPiiUpdateMessage = CustomerPiiUpdateMessageFaker
            .GenerateUpdateMessageWithNoAddress();


        // Act
        var result = customerPiiUpdateMessage.HasPrimaryAddress();

        //Assert
        Assert.False(result);
    }
}
