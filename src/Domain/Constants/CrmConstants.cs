namespace Domain.Constants;

public static class CrmConstants
{
    public const string GetUserAndPersonAccountsQuery = @"
    query UserPerson($userId: Int, $personIds: [ID]) {
        userPerson(userPersonQueryInput: {userID: $userId}) {
            person {
                addressTypeId
                birthDate
                created
                customerUuid
                domesticAddress{
                    city
                    postalCode
                    province
                    provinceCode
                    provinceId
                    streetDirection
                    streetName
                    streetNumber
                    streetSuffix
                    streetType
                    unitNumber
                }
                email
                firstName
                internationalAddress{
                    addressLine1
                    addressLine2
                    addressLine3
                    addressTypeId
                    bpsCountryCode
                    countryCode
                    countryId
                    countryName
                    isIRSTreatyCountry
                    ismCountryCode
                    ismResidenceCode
                    isoCountryCode
                    postalCode
                    provinceState
                }
                lastName
                middleName
                phoneNumbers{
                    phoneNumber
                    phoneNumberRank
                    phoneNumberType
                    phoneNumberTypeId
                }
                updated
            }
        }
        personAccounts(personAccountQueryInput: {personIds: $personIds}) {
            accountNumber
            effectiveDate
            accountStatusId
        }
    }";
}
