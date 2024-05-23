namespace Unit.Constants;

public static class CustomerPiiSubscribeMessageConstant
{
    public const string SubscribeMessageCustomer =@"
    {
        ""customer"":
        {
            ""masterProfileId"":""9484d83c-f310-5e3e-8f17-56e3883404ea"",
            ""revision"":""20240220143346613"",
            ""isIdentityVerified"":true,
            ""isQuestradeEmployee"":false,
            ""isDeactivated"":false,
            ""id"":""81d12b62-3f7d-46b5-92f8-fb886a028df0""
        }
    }";

    public const string SubscribeMessageDeltaChangesComplexValue = @"
    {
        ""deltaChanges"":[
        {
            ""op"":""add"",
            ""path"":""/Profile/Personal/ElectronicContacts"",
            ""value"":[{
                    ""id"":""primaryemail-81d12b62-3f7d-46b5-92f8-fb886a028df0"",
                    ""type"":""Email"",
                    ""subtype"":""Email"",
                    ""value"":""worker@questrade.com"",
                    ""status"":""Active"",
                    ""updatedAt"":""2024-02-20T09:33:40.646Z"",
                    ""isVerified"":true
            }]
        },
        {
            ""op"":""remove"",
            ""path"":""/Profile/Personal/ElectronicContacts"",
            ""value"":[{
                ""id"":""primaryemail-81d12b62-3f7d-46b5-92f8-fb886a028df0"",
                ""type"":""Email"",
                ""subtype"":""Email"",
                ""value"":""worker2@questrade.com"",
                ""status"":""Active"",
                ""updatedAt"":""2024-02-20T09:32:56.396Z"",
                ""isVerified"":true
            }]
        }]
    }";

    public const string SubscribeMessageDeltaChangesPrimitiveValue = @"
    {
        ""deltaChanges"":[
            {
            ""op"":""remove"",
            ""path"":""/Profile/Financial/AnnualIncome"",
            ""value"":1
            },
            {
            ""op"":""add"",
            ""path"":""/Profile/Financial/AnnualIncome"",
            ""value"":1234553
            }
        ]
    }";

    public const string SubscribeMessageEmploymentItems = @"
    {
        ""customer"":
        {
            ""profile"":
            {
                ""employment"":
                {
                    ""employmentItems"":
                    [
                        {
                            ""id"":""9979_81d12b62-3f7d-46b5-92f8-fb886a028df0"",
                            ""employmentId"":""9979"",
                            ""employerName"":""Business Co."",
                            ""employmentType"":""Employed"",
                            ""employmentSubType"":""Unknown"",
                            ""jobTitle"":""Office Manager"",
                            ""typeOfBusiness"":""Professional Services"",
                            ""incomeSource"":{""name"":""Parents""},
                            ""employmentAffiliation"":{""isAffiliated"":true},
                            ""employmentSpouseAffiliation"":{""isAffiliated"":true},
                            ""workPhoneNumber"":""4165551234 x 123"",
                            ""employerAddress"":{
                                ""id"":""employeraddress-32e8b97b-0243-4961-abc7-f7c20a13ad6d"",
                                ""formattedAddress"":[
                                    ""305-5650 Yonge ST"",
                                    ""Toronto ON M2M4G3""
                                ],
                                ""postalCode"":""M2M4G3"",
                                ""country"":""Canada"",
                                ""countryCode"":""CA"",
                                ""type"":""Employer address"",
                                ""province"":""Ontario"",
                                ""provinceCode"":""ON"",
                                ""city"":""Toronto"",
                                ""streetName"":""Yonge"",
                                ""streetType"":""ST"",
                                ""streetNumber"":""5650"",
                                ""unitNumber"":""305"",
                                ""status"":""Active"",
                                ""updatedAt"":""2024-01-12T17:13:30.082Z""
                            },
                            ""employmentPep"":
                            {
                                ""isPoliticallyExposed"":true,
                                ""politicallyExposedPersons"":
                                [
                                    {
                                        ""politicallyExposedId"":""1561808"",
                                        ""isPoliticallyExposed"":true,
                                        ""politicallyExposedName"":""test test"",
                                        ""politicallyExposedReason"":""test"",
                                        ""politicallyExposedSourceOfFunds"":""test"",
                                        ""politicallyExposedSourceOfWealth"":""test""
                                    }
                                ]
                            },
                            ""employmentInsider"":{""isInsider"":true},
                            ""updatedAt"":""2023-10-30T17:02:02.91Z""
                        }
                    ]
                }
            }
        }
    }";

    public const string SubscribeMessageFinancial = @"
    {
        ""customer"":
        {
            ""profile"":
            {
                ""financial"":
                {
                    ""annualIncome"":1.00,
                    ""assets"":1.00,
                    ""liabilities"":1.00,
                    ""liquidAssets"":1.00,
                    ""netWorth"":-1
                }
            }
        }
    }";

    public const string SubscribeMessagePersonal = @"
    {
        ""customer"":
        {
            ""profile"":
            {
                ""personal"":
                {
                    ""title"":""Unknown"",
                    ""firstName"":""First"",
                    ""middleName"":""Middle"",
                    ""lastName"":""Last"",
                    ""fullName"":""First Middle Last"",
                    ""dateOfBirth"":""1980-06-12T00:00:00Z"",
                    ""gender"":""Unknown"",
                    ""maritalStatus"":""Married"",
                    ""addresses"":
                    [
                        {
                            ""id"":""primary-81d12b62-3f7d-46b5-92f8-fb886a028df0"",
                            ""formattedAddress"":
                            [
                                ""430, Rua SJ"",
                                ""CTW"",
                                ""99887-766""
                            ],
                            ""postalCode"":""99887-766"",
                            ""country"":""Brazil"",
                            ""countryCode"":""BR"",
                            ""type"":""Primary"",
                            ""province"":""Santa Catarina"",
                            ""provinceCode"":""SC"",
                            ""city"":""Somewhere"",
                            ""streetName"":""St. Someplace"",
                            ""streetType"":""AV"",
                            ""streetNumber"":""190"",
                            ""unitNumber"":""200"",
                            ""status"":""Active"",
                            ""updatedAt"":""2023-03-09T13:27:39.802Z""
                        }
                    ],
                    ""electronicContacts"":
                    [
                        {
                            ""id"":""primaryemail-81d12b62-3f7d-46b5-92f8-fb886a028df0"",
                            ""type"":""Email"",
                            ""subtype"":""Email"",
                            ""value"":""worker@questrade.com"",
                            ""status"":""Active"",
                            ""updatedAt"":""2024-02-20T09:33:40.646Z"",
                            ""isVerified"":true
                        }
                    ],
                    ""phones"":
                    [
                        {
                            ""id"":""telephonedaytime-81d12b62-3f7d-46b5-92f8-fb886a028df0"",
                            ""phoneNumber"":""123456789"",
                            ""phoneNumberType"":""Telephone Daytime"",
                            ""reasonForInternationalPhone"":"""",
                            ""status"":""Active"",
                            ""updatedAt"":""2023-11-10T10:59:41:00Z"",
                            ""isVerified"":true
                        },
                        {
                            ""id"":""primary-81d12b62-3f7d-46b5-92f8-fb886a028df0"",
                            ""phoneNumber"":""123456789"",
                            ""phoneNumberType"":""Primary"",
                            ""reasonForInternationalPhone"":"""",
                            ""status"":""Active"",
                            ""updatedAt"":""2023-11-10T10:59:41:00Z"",
                            ""isVerified"":true
                        }
                    ],
                    ""relationships"":[
                        {
                            ""id"":""70883101_32e8b97b-0243-4961-abc7-f7c20a13ad6d"",
                            ""title"":""Unknown"",
                            ""type"":""Spouse"",
                            ""relationshipId"":""70883101"",
                            ""firstName"":""Cassandra"",
                            ""lastName"":""Webb"",
                            ""dateOfBirth"":""1989-10-11T00:00:00Z"",
                            ""updatedAt"":""2023-10-17T16:41:53.677Z""
                        }
                    ],
                    ""lastAnnualKycUpdate"":""2023-10-30T17:02:37.3Z""
                }
            }
        }
    }";

    public const string SubscribeMessageRelatedReferences = @"
    {
        ""customer"":
        {
            ""relatedReferences"":
            [
                {""id"":""70871407"",""name"":""CRM"",""attributeName"":""PersonID""},
                {""id"":""81d12b62-3f7d-46b5-92f8-fb886a028df0"",""name"":""CRM"",""attributeName"":""CustomerUUID""},
                {""id"":""7131935"",""name"":""CRM"",""attributeName"":""UserID""}
            ]
        }
    }";

    public const string SubscribeMessageTax = @"
    {
        ""customer"":
            {
                ""profile"":
                {
                    ""tax"":
                    {
                        ""canada"":
                        {
                            ""sin"":""446849812"",
                            ""consentToUseSIN"":true,
                            ""isTaxResident"":true,
                            ""isResident"":true,
                            ""sinExpiryDate"": ""2022-01-13T21:18:27.020Z""
                        },
                        ""usa"":
                        {
                            ""ssn"":""446849813"",
                            ""isBornInUS"":true,
                            ""isPersonUS"":true,
                            ""isRenouncedCitizenship"":true
                        },
                        ""otherTaxJurisdictions"":
                        [
                            {
                                ""countryCode"":""CA"",
                                ""tinAbsentReasonCategory"":""tinAbsentReasonCategory"",
                                ""tinAbsentReason"":""tinAbsentReason"",
                                ""tin"":""tin""
                            }
                        ],
                        ""crsCertificationAccepted"":true
                    }
                }
            }
    }";
}
