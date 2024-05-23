using Domain.Models;
using Infra.Subscriber.Extensions;
using Unit.Fixture;
using Xunit;

namespace Unit.Subscriber.Extensions;

public class EmailExtensionHandlePhoneChangeTests
{
    private readonly CustomerProfileUpdatedMessageFixture _customerProfileUpdatedMessageFixture;

    public EmailExtensionHandlePhoneChangeTests()
    {
        _customerProfileUpdatedMessageFixture = new CustomerProfileUpdatedMessageFixture();
    }

    [Fact]
    public void HandleEmailChanges_ShouldReturnValidRequest_WhenEmailHaveChanged()
    {
        // Arrange
        var profileUpdated = _customerProfileUpdatedMessageFixture.GenerateValidUpdatedCustomerMessageWithEmail();

        // Act
        var response = profileUpdated.HandleEmailChanges();

        // Assert
        Assert.NotNull(response);

        Assert.IsType<CustomerProfileEmailUpdatedRequest>(response);

        var requestUpdated = (CustomerProfileEmailUpdatedRequest)response!;

        Assert.Equal(profileUpdated.UserId, requestUpdated.CrmUserId);

        Assert.NotEmpty(requestUpdated.Email);
    }

    [Fact]
    public void HandleEmailChanges_ShouldReturnNull_WhenNoEmailHaveChanged()
    {
        // Arrange
        var profileUpdated = _customerProfileUpdatedMessageFixture.GenerateInvalidUpdatedCustomerMessageWithName();

        // Act
        var response = profileUpdated.HandleEmailChanges();

        // Assert
        Assert.Null(response);
    }
}
