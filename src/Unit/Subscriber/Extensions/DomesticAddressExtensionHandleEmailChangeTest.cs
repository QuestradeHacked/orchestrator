using Domain.Models;
using Infra.Subscriber.Extensions;
using Unit.Fixture;
using Xunit;

namespace Unit.Subscriber.Extensions;

public class DomesticAddressExtensionHandleEmailChangeTest
{
    private readonly CustomerProfileUpdatedMessageFixture _customerProfileUpdatedMessageFixture;

    public DomesticAddressExtensionHandleEmailChangeTest()
    {
        _customerProfileUpdatedMessageFixture = new CustomerProfileUpdatedMessageFixture();
    }

    [Fact]
    public void HandleDomesticAddressChanges_ShouldReturnValidRequest_WhenDomesticAddressIsValid()
    {
        // Arrange
        var profileUpdated = _customerProfileUpdatedMessageFixture.GenerateValidUpdatedCustomerMessageWithValidDomesticAddress();

        // Act
        var response = profileUpdated.HandleDomesticAddressChanges();

        // Assert
        Assert.NotNull(response);
        Assert.NotEmpty(response);
        Assert.IsType<List<IRequestOrchestrator>>(response);
    }

    [Fact]
    public void HandleDomesticAddressChanges_ShouldReturnValidRequest_WhenDomesticAddressIsInvalid()
    {
        // Arrange
        var profileUpdated = _customerProfileUpdatedMessageFixture.GenerateValidUpdatedCustomerMessageWithInvalidDomesticAddress();

        // Act
        var response = profileUpdated.HandleDomesticAddressChanges();

        // Assert
        Assert.NotNull(response);
        Assert.Empty(response);
    }

    [Fact]
    public void HandleDomesticAddressChanges_ShouldReturnInvalidRequest_WhenDomesticAddressIsNull()
    {
        // Arrange
        var profileUpdated = _customerProfileUpdatedMessageFixture.GenerateInvalidUpdatedCustomerMessageWithNullDomesticAddress();

        // Act
        var response = profileUpdated.HandleDomesticAddressChanges();

        // Assert
        Assert.NotNull(response);
        Assert.Empty(response);
    }
}
