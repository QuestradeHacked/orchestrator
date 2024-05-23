using Domain.Models.Pii;
using FluentAssertions;
using Unit.Constants;
using System.Text.Json;
using Xunit;

namespace Unit.Domain.Models.Pii;

public class CustomerPiiUpdateMessageTest
{
    [Fact]
    public void DeserializeObject_ShouldParseJSON_WhenCustomerIsLoaded()
    {
        var customerPiiUpdateMessage = ParseJsonToCustomerPii(CustomerPiiSubscribeMessageConstant.SubscribeMessageCustomer);

        customerPiiUpdateMessage.Customer.Should().NotBeNull();
        customerPiiUpdateMessage.Customer!.MasterProfileId.Should().Be("9484d83c-f310-5e3e-8f17-56e3883404ea");
        customerPiiUpdateMessage.Customer.Revision.Should().Be("20240220143346613");
        customerPiiUpdateMessage.Customer.IsIdentityVerified.Should().Be(true);
        customerPiiUpdateMessage.Customer.IsQuestradeEmployee.Should().Be(false);
        customerPiiUpdateMessage.Customer.IsDeactivated.Should().Be(false);
    }

    [Fact]
    public void DeserializeObject_ShouldParseJSON_WhenDeltaChangesWithComplexValueIsLoaded()
    {
        var customerPiiUpdateMessageUpdatedMessage = ParseJsonToCustomerPii(CustomerPiiSubscribeMessageConstant.SubscribeMessageDeltaChangesComplexValue);

        var deltaChanges = customerPiiUpdateMessageUpdatedMessage.DeltaChanges;

        deltaChanges.Should().HaveCount(2);

        deltaChanges[0].Operation.Should().Be("add");
        deltaChanges[0].Path.Should().Be("/Profile/Personal/ElectronicContacts");

        deltaChanges[1].Operation.Should().Be("remove");
        deltaChanges[1].Path.Should().Be("/Profile/Personal/ElectronicContacts");
    }

    [Fact]
    public void DeserializeObject_ShouldParseJSON_WhenDeltaChangesWithPrimitiveValueIsLoaded()
    {
        var customerPiiUpdateMessage = ParseJsonToCustomerPii(CustomerPiiSubscribeMessageConstant.SubscribeMessageDeltaChangesPrimitiveValue);

        var deltaChanges = customerPiiUpdateMessage.DeltaChanges;

        deltaChanges.Should().HaveCount(2);

        deltaChanges[0].Operation.Should().Be("remove");
        deltaChanges[0].Path.Should().Be("/Profile/Financial/AnnualIncome");

        deltaChanges[1].Operation.Should().Be("add");
        deltaChanges[1].Path.Should().Be("/Profile/Financial/AnnualIncome");

    }

    [Fact]
    public void DeserializeObject_ShouldParseJSON_WhenEmploymentItemsIsLoaded()
    {
        var customerPiiUpdateMessage = ParseJsonToCustomerPii(CustomerPiiSubscribeMessageConstant.SubscribeMessageEmploymentItems);

        customerPiiUpdateMessage.Customer!.Profile!.Employment!.EmploymentItems.Should().HaveCount(1);

        var employmentItem = customerPiiUpdateMessage.Customer.Profile.Employment.EmploymentItems[0];
        employmentItem.Id.Should().Be("9979_81d12b62-3f7d-46b5-92f8-fb886a028df0");
        employmentItem.EmploymentId.Should().Be("9979");
        employmentItem.EmployerName.Should().Be("Business Co.");
        employmentItem.EmploymentType.Should().Be("Employed");
        employmentItem.EmploymentSubType.Should().Be("Unknown");
        employmentItem.JobTitle.Should().Be("Office Manager");
        employmentItem.TypeOfBusiness.Should().Be("Professional Services");
        employmentItem.IncomeSource.Should().NotBeNull();
        employmentItem.IncomeSource!.Name.Should().Be("Parents");
        employmentItem.EmploymentAffiliation!.IsAffiliated.Should().BeTrue();
        employmentItem.EmploymentSpouseAffiliation!.IsAffiliated.Should().BeTrue();
        employmentItem.WorkPhoneNumber.Should().Be("4165551234 x 123");

        employmentItem.EmployerAddress.Should().NotBeNull();
        employmentItem.EmployerAddress!.Id.Should().Be("employeraddress-32e8b97b-0243-4961-abc7-f7c20a13ad6d");
        employmentItem.EmployerAddress.FormattedAddress.Should().HaveCount(2);
        employmentItem.EmployerAddress.PostalCode.Should().Be("M2M4G3");
        employmentItem.EmployerAddress.Country.Should().Be("Canada");
        employmentItem.EmployerAddress.CountryCode.Should().Be("CA");
        employmentItem.EmployerAddress.Type.Should().Be("Employer address");
        employmentItem.EmployerAddress.Province.Should().Be("Ontario");
        employmentItem.EmployerAddress.ProvinceCode.Should().Be("ON");
        employmentItem.EmployerAddress.City.Should().Be("Toronto");
        employmentItem.EmployerAddress.StreetName.Should().Be("Yonge");
        employmentItem.EmployerAddress.StreetType.Should().Be("ST");
        employmentItem.EmployerAddress.StreetNumber.Should().Be("5650");
        employmentItem.EmployerAddress.UnitNumber.Should().Be("305");
        employmentItem.EmployerAddress.Status.Should().Be("Active");
        employmentItem.EmployerAddress.UpdatedAt.Should().Be("2024-01-12T17:13:30.082Z");

        employmentItem.EmploymentPep.Should().NotBeNull();
        employmentItem.EmploymentPep!.IsPoliticallyExposed.Should().BeTrue();
        employmentItem.EmploymentPep.PoliticallyExposedPersons.Should().HaveCount(1);
        employmentItem.EmploymentPep.PoliticallyExposedPersons[0].PoliticallyExposedId.Should().Be("1561808");
        employmentItem.EmploymentPep.PoliticallyExposedPersons[0].IsPoliticallyExposed.Should().BeTrue();
        employmentItem.EmploymentPep.PoliticallyExposedPersons[0].PoliticallyExposedName.Should().Be("test test");
        employmentItem.EmploymentPep.PoliticallyExposedPersons[0].PoliticallyExposedReason.Should().Be("test");
        employmentItem.EmploymentPep.PoliticallyExposedPersons[0].PoliticallyExposedSourceOfFunds.Should().Be("test");
        employmentItem.EmploymentPep.PoliticallyExposedPersons[0].PoliticallyExposedSourceOfWealth.Should().Be("test");
        employmentItem.EmploymentInsider.Should().NotBeNull();
        employmentItem.EmploymentInsider!.IsInsider.Should().BeTrue();
        employmentItem.UpdatedAt!.Should().Be("2023-10-30T17:02:02.91Z");
    }

    [Fact]
    public void DeserializeObject_ShouldParseJSON_WhenFinancialIsLoaded()
    {
        var customerPiiUpdateMessage = ParseJsonToCustomerPii(CustomerPiiSubscribeMessageConstant.SubscribeMessageFinancial);

        var financial = customerPiiUpdateMessage.Customer!.Profile?.Financial!;
        financial.Should().NotBeNull();
        financial.AnnualIncome.Should().Be(1);
        financial.Assets.Should().Be(1);
        financial.Liabilities.Should().Be(1);
        financial.LiquidAssets.Should().Be(1);
        financial.NetWorth.Should().Be(-1);
    }

    [Fact]
    public void DeserializeObject_ShouldParseJSON_WhenPersonalIsLoaded()
    {
        var customerPiiUpdateMessage = ParseJsonToCustomerPii(CustomerPiiSubscribeMessageConstant.SubscribeMessagePersonal);

        var personal = customerPiiUpdateMessage.Customer!.Profile!.Personal;

        personal.Should().NotBeNull();
        personal!.Title.Should().Be("Unknown");
        personal.FirstName.Should().Be("First");
        personal.MiddleName.Should().Be("Middle");
        personal.LastName.Should().Be("Last");
        personal.FullName.Should().Be("First Middle Last");
        personal.DateOfBirth.Should().Be("1980-06-12T00:00:00Z");
        personal.Gender.Should().Be("Unknown");
        personal.MaritalStatus.Should().Be("Married");

        personal.Addresses.Should().HaveCount(1);
        personal.Addresses![0].Id.Should().Be("primary-81d12b62-3f7d-46b5-92f8-fb886a028df0");
        personal.Addresses[0].FormattedAddress.Should().HaveCount(3);
        personal.Addresses[0].PostalCode.Should().Be("99887-766");
        personal.Addresses[0].Country.Should().Be("Brazil");
        personal.Addresses[0].CountryCode.Should().Be("BR");
        personal.Addresses[0].Type.Should().Be("Primary");
        personal.Addresses[0].Province.Should().Be("Santa Catarina");
        personal.Addresses[0].ProvinceCode.Should().Be("SC");
        personal.Addresses[0].City.Should().Be("Somewhere");
        personal.Addresses[0].StreetName.Should().Be("St. Someplace");
        personal.Addresses[0].StreetType.Should().Be("AV");
        personal.Addresses[0].StreetNumber.Should().Be("190");
        personal.Addresses[0].UnitNumber.Should().Be("200");

        personal.Addresses[0].Status.Should().Be("Active");
        personal.Addresses[0].UpdatedAt.Should().Be("2023-03-09T13:27:39.802Z");

        personal.ElectronicContacts.Should().HaveCount(1);
        personal.ElectronicContacts![0].Id.Should().Be("primaryemail-81d12b62-3f7d-46b5-92f8-fb886a028df0");
        personal.ElectronicContacts![0].Type.Should().Be("Email");
        personal.ElectronicContacts![0].Subtype.Should().Be("Email");
        personal.ElectronicContacts![0].Value.Should().Be("worker@questrade.com");
        personal.ElectronicContacts![0].Status.Should().Be("Active");
        personal.ElectronicContacts![0].UpdatedAt.Should().Be("2024-02-20T09:33:40.646Z");
        personal.ElectronicContacts![0].IsVerified.Should().Be(true);

        personal.Phones.Should().HaveCount(2);
        personal.Phones![0].Id.Should().Be("telephonedaytime-81d12b62-3f7d-46b5-92f8-fb886a028df0");
        personal.Phones![0].PhoneNumber.Should().Be("123456789");
        personal.Phones![0].PhoneNumberType.Should().Be("Telephone Daytime");
        personal.Phones![0].ReasonForInternationalPhone.Should().Be(string.Empty);
        personal.Phones![0].Status.Should().Be("Active");
        personal.Phones![0].UpdatedAt.Should().Be("2023-11-10T10:59:41:00Z", because: "the date should be in ISO 8601 format without milliseconds");
        personal.Phones![0].IsVerified.Should().Be(true);

        personal.Phones![1].Id.Should().Be("primary-81d12b62-3f7d-46b5-92f8-fb886a028df0");
        personal.Phones![1].PhoneNumber.Should().Be("123456789");
        personal.Phones![1].PhoneNumberType.Should().Be("Primary");
        personal.Phones![1].ReasonForInternationalPhone.Should().Be(string.Empty);
        personal.Phones![1].Status.Should().Be("Active");
        personal.Phones![1].UpdatedAt.Should().Be("2023-11-10T10:59:41:00Z", because: "the date should be in ISO 8601 format without milliseconds");
        personal.Phones![1].IsVerified.Should().Be(true);

        personal.Relationships.Should().HaveCount(1);
        personal.Relationships[0].Id.Should().Be("70883101_32e8b97b-0243-4961-abc7-f7c20a13ad6d");
        personal.Relationships[0].Title.Should().Be("Unknown");
        personal.Relationships[0].Type.Should().Be("Spouse");
        personal.Relationships[0].RelationshipId.Should().Be("70883101");
        personal.Relationships[0].FirstName.Should().Be("Cassandra");
        personal.Relationships[0].LastName.Should().Be("Webb");
        personal.Relationships[0].DateOfBirth.Should().Be("1989-10-11T00:00:00Z");
        personal.Relationships[0].UpdatedAt.Should().Be("2023-10-17T16:41:53.677Z");

        personal.LastAnnualKycUpdate.Should().Be("2023-10-30T17:02:37.3Z");
    }

    [Fact]
    public void DeserializeObject_ShouldParseJSON_WhenRelatedReferencesIsLoaded()
    {
        var customerPiiUpdateMessage = ParseJsonToCustomerPii(CustomerPiiSubscribeMessageConstant.SubscribeMessageRelatedReferences);

        var relatedReferences = customerPiiUpdateMessage.Customer!.RelatedReferences;

        relatedReferences.Should().NotBeNull();
        relatedReferences.Should().HaveCount(3);

        relatedReferences[0].Id.Should().Be("70871407");
        relatedReferences[0].Name.Should().Be("CRM");
        relatedReferences[0].AttributeName.Should().Be("PersonID");

        relatedReferences[1].Id.Should().Be("81d12b62-3f7d-46b5-92f8-fb886a028df0");
        relatedReferences[1].Name.Should().Be("CRM");
        relatedReferences[1].AttributeName.Should().Be("CustomerUUID");

        relatedReferences[2].Id.Should().Be("7131935");
        relatedReferences[2].Name.Should().Be("CRM");
        relatedReferences[2].AttributeName.Should().Be("UserID");
    }

    [Fact]
    public void DeserializeObject_ShouldParseJSON_WhenTaxIsLoaded()
    {
        var customerPiiUpdateMessage = ParseJsonToCustomerPii(CustomerPiiSubscribeMessageConstant.SubscribeMessageTax);

        var tax = customerPiiUpdateMessage.Customer?.Profile?.Tax;
        tax.Should().NotBeNull();

        tax!.Canada.Should().NotBeNull();
        tax.Canada!.SIN.Should().Be("446849812");
        tax.Canada.ConsentToUseSIN.Should().BeTrue();
        tax.Canada.IsTaxResident.Should().BeTrue();
        tax.Canada.IsResident.Should().BeTrue();
        tax.Canada.SINExpiryDate.Should().Be("2022-01-13T21:18:27.020Z");

        tax.USA.Should().NotBeNull();
        tax.USA!.Ssn.Should().Be("446849813");
        tax.USA.IsBornInUs.Should().BeTrue();
        tax.USA.IsPersonUs.Should().BeTrue();
        tax.USA.IsRenouncedCitizenship.Should().BeTrue();

        tax.OtherTaxJurisdictions.Should().HaveCount(1);
        tax.OtherTaxJurisdictions[0].CountryCode.Should().Be("CA");
        tax.OtherTaxJurisdictions[0].TinAbsentReasonCategory.Should().Be("tinAbsentReasonCategory");
        tax.OtherTaxJurisdictions[0].TinAbsentReason.Should().Be("tinAbsentReason");
        tax.OtherTaxJurisdictions[0].Tin.Should().Be("tin");

        tax.CrsCertificationAccepted.Should().Be(true);
    }

    private static CustomerPiiUpdateMessage ParseJsonToCustomerPii(string jsonMessage)
    {
        var customerPiiUpdateMessage = JsonSerializer.Deserialize<CustomerPiiUpdateMessage>(jsonMessage);

        customerPiiUpdateMessage.Should().NotBeNull();

        return customerPiiUpdateMessage!;
    }
}
