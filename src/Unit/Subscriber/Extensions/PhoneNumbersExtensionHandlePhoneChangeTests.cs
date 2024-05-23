using Domain.Models;
using Infra.Subscriber.Extensions;
using MediatR;
using Unit.Fixture;
using Xunit;

namespace Unit.Subscriber.Extensions;

public class PhoneNumbersExtensionHandlePhoneChangeTests
{
    private readonly CustomerProfileUpdatedMessageFixture _customerProfileUpdatedMessageFixture = new();

    [Fact]
    public void HandlePhoneChangesForIdentityIntelligence_ShouldReturnValidRequest_WhenOnePhoneNumberHaveChanged()
    {
        // Arrange
        var profileUpdated = _customerProfileUpdatedMessageFixture.GenerateValidUpdatedCustomerMessageWithOnePhoneNumber();

        // Act
        var response = profileUpdated.HandlePhoneChangesForIdentityIntelligence();

        // Assert
        AssertSuccessIdentityUpdatedRequest(response, profileUpdated);
    }

    [Fact]
    public void HandlePhoneChangesForIdentityIntelligence_ShouldReturnValidRequest_WhenTwoPhonesHaveChanged()
    {
        // Arrange
        var profileUpdated = _customerProfileUpdatedMessageFixture.GenerateValidUpdatedCustomerMessageWithTwoPhoneNumbers();

        // Act
        var response = profileUpdated.HandlePhoneChangesForIdentityIntelligence();

        // Assert
        AssertSuccessIdentityUpdatedRequest(response, profileUpdated);
    }

    [Fact]
    public void HandlePhoneChangesForIdentityIntelligence_ShouldReturnNull_WhenPhoneNumbersAreNull()
    {
        // Arrange
        var profileUpdated = _customerProfileUpdatedMessageFixture.GenerateInvalidUpdatedCustomerMessageWithNullPhoneNumbers();

        // Act
        var response = profileUpdated.HandlePhoneChangesForIdentityIntelligence();

        // Assert
        AssertNothingToProcess(response);
    }

    [Fact]
    public void HandlePhoneChangesForIdentityIntelligence_ShouldReturnNull_WhenPhoneNumbersAreEmpty()
    {
        // Arrange
        var profileUpdated = _customerProfileUpdatedMessageFixture.GenerateInvalidUpdatedCustomerMessageWithEmptyPhoneNumbers();

        // Act
        var response = profileUpdated.HandlePhoneChangesForIdentityIntelligence();

        // Assert
        AssertNothingToProcess(response);
    }

    [Fact]
    public void HandlePhoneChangesForIdentityIntelligence_ShouldReturnNull_WhenWorkPhoneNumbersAreChanged()
    {
        // Arrange
        var profileUpdated = _customerProfileUpdatedMessageFixture.GenerateInvalidUpdatedCustomerMessageWithWorkPhoneNumbers();

        // Act
        var response = profileUpdated.HandlePhoneChangesForIdentityIntelligence();

        // Assert
        AssertNothingToProcess(response);
    }

    [Fact]
    public void HandlePhoneChangesForIdentityIntelligence_ShouldReturnValid_WhenHaveOneWorkPhoneNumberAndOnePersonalPhoneNumberChanged()
    {
        // Arrange
        var profileUpdated = _customerProfileUpdatedMessageFixture.GenerateValidUpdatedCustomerMessageWithOneWorkPhoneNumberAndOnePersonalPhoneNumber();

        // Act
        var response = profileUpdated.HandlePhoneChangesForIdentityIntelligence() as CustomerProfileIdentityUpdatedRequest;

        // Assert
        AssertSuccessIdentityUpdatedRequest(response, profileUpdated);
        Assert.Single(response!.PhoneNumbers);
    }

    [Fact]
    public void HandlePhoneChangesForIdentityIntelligence_ShouldReturnNull_WhenEmailHaveUpdated()
    {
        // Arrange
        var profileUpdated = _customerProfileUpdatedMessageFixture.GenerateInvalidUpdatedCustomerMessageWithEmail();

        // Act
        var response = profileUpdated.HandlePhoneChangesForIdentityIntelligence();

        // Assert
        AssertNothingToProcess(response);
    }

    [Fact]
    public void HandlePhoneChangesForIdentityIntelligence_ShouldReturnNull_WhenNameHaveUpdated()
    {
        // Arrange
        var profileUpdated = _customerProfileUpdatedMessageFixture.GenerateInvalidUpdatedCustomerMessageWithName();

        // Act
        var response = profileUpdated.HandlePhoneChangesForIdentityIntelligence();

        // Assert
        AssertNothingToProcess(response);
    }


    [Fact]
    public void HandlePhoneChangesForSimIntelligence_ShouldReturnValidRequest_WhenOnePhoneNumberHaveChanged()
    {
        // Arrange
        var profileUpdated = _customerProfileUpdatedMessageFixture.GenerateValidUpdatedCustomerMessageWithOnePhoneNumber();

        // Act
        var response = profileUpdated.HandlePhoneChangesForSimIntelligence();

        // Assert
        AssertSuccessPhoneUpdatedRequest(response, profileUpdated);
    }

    [Fact]
    public void HandlePhoneChangesForSimIntelligence_ShouldReturnValidRequest_WhenTwoPhonesHaveChanged()
    {
        // Arrange
        var profileUpdated = _customerProfileUpdatedMessageFixture.GenerateValidUpdatedCustomerMessageWithTwoPhoneNumbers();

        // Act
        var response = profileUpdated.HandlePhoneChangesForSimIntelligence();

        // Assert
        AssertSuccessPhoneUpdatedRequest(response, profileUpdated);
    }

    [Fact]
    public void HandlePhoneChangesForSimIntelligence_ShouldReturnNull_WhenPhoneNumbersAreNull()
    {
        // Arrange
        var profileUpdated = _customerProfileUpdatedMessageFixture.GenerateInvalidUpdatedCustomerMessageWithNullPhoneNumbers();

        // Act
        var response = profileUpdated.HandlePhoneChangesForSimIntelligence();

        // Assert
        AssertNothingToProcess(response);
    }

    [Fact]
    public void HandlePhoneChangesForSimIntelligence_ShouldReturnNull_WhenPhoneNumbersAreEmpty()
    {
        // Arrange
        var profileUpdated = _customerProfileUpdatedMessageFixture.GenerateInvalidUpdatedCustomerMessageWithEmptyPhoneNumbers();

        // Act
        var response = profileUpdated.HandlePhoneChangesForSimIntelligence();

        // Assert
        AssertNothingToProcess(response);
    }

    [Fact]
    public void HandlePhoneChangesForSimIntelligence_ShouldReturnNull_WhenWorkPhoneNumbersAreChanged()
    {
        // Arrange
        var profileUpdated = _customerProfileUpdatedMessageFixture.GenerateInvalidUpdatedCustomerMessageWithWorkPhoneNumbers();

        // Act
        var response = profileUpdated.HandlePhoneChangesForSimIntelligence();

        // Assert
        AssertNothingToProcess(response);
    }

    [Fact]
    public void HandlePhoneChangesForSimIntelligence_ShouldReturnValid_WhenHaveOneWorkPhoneNumberAndOnePersonalPhoneNumberChanged()
    {
        // Arrange
        var profileUpdated = _customerProfileUpdatedMessageFixture.GenerateValidUpdatedCustomerMessageWithOneWorkPhoneNumberAndOnePersonalPhoneNumber();

        // Act
        var response = profileUpdated.HandlePhoneChangesForSimIntelligence() as CustomerProfilePhoneUpdatedRequest;

        // Assert
        AssertSuccessPhoneUpdatedRequest(response, profileUpdated);
        Assert.Single(response!.PhoneNumbers);
    }

    [Fact]
    public void HandlePhoneChangesForSimIntelligence_ShouldReturnNull_WhenEmailHaveUpdated()
    {
        // Arrange
        var profileUpdated = _customerProfileUpdatedMessageFixture.GenerateInvalidUpdatedCustomerMessageWithEmail();

        // Act
        var response = profileUpdated.HandlePhoneChangesForSimIntelligence();

        // Assert
        AssertNothingToProcess(response);
    }

    [Fact]
    public void HandlePhoneChangesForSimIntelligence_ShouldReturnNull_WhenNameHaveUpdated()
    {
        // Arrange
        var profileUpdated = _customerProfileUpdatedMessageFixture.GenerateInvalidUpdatedCustomerMessageWithName();

        // Act
        var response = profileUpdated.HandlePhoneChangesForSimIntelligence();

        // Assert
        AssertNothingToProcess(response);
    }

    private static void AssertSuccessPhoneUpdatedRequest(IRequest? request, CustomerProfileUpdatedMessage message)
    {
        Assert.NotNull(request);

        Assert.IsType<CustomerProfilePhoneUpdatedRequest>(request);

        var requestPhoneUpdated = (CustomerProfilePhoneUpdatedRequest)request!;

        Assert.Equal(message.UserId, requestPhoneUpdated.CrmUserId);

        Assert.NotEmpty(requestPhoneUpdated.PhoneNumbers);
    }

    private static void AssertSuccessIdentityUpdatedRequest(IRequest? request, CustomerProfileUpdatedMessage message)
    {
        Assert.NotNull(request);

        Assert.IsType<CustomerProfileIdentityUpdatedRequest>(request);

        var requestPhoneUpdated = (CustomerProfileIdentityUpdatedRequest)request!;

        Assert.Equal(message.UserId, requestPhoneUpdated.CrmUserId);

        Assert.NotEmpty(requestPhoneUpdated.PhoneNumbers);
    }

    private static void AssertNothingToProcess(IBaseRequest? request)
    {
        Assert.Null(request);
    }
}
