using Domain.Models.CRM;
using GraphQL;

namespace Unit.Faker;

public static class GraphQLResponseFaker
{
    public static GraphQLResponse<CrmResponse> GenerateValidResponse()
    {
        return new GraphQLResponse<CrmResponse>()
        {
            Data = new CrmResponse
            {
                UserPerson = new List<UserPersons>()
                {
                    new UserPersons()
                    {
                        Persons = new List<CrmPerson>()
                        {
                            new Bogus.Faker<CrmPerson>()
                                .RuleFor(u => u.FirstName, p => p.Person.FirstName)
                        }
                    }
                },
                PersonAccounts = new List<PersonAccount>()
                {
                    new Bogus.Faker<PersonAccount>()
                        .RuleFor(u => u.EffectiveDate, f => f.Date.Past())
                }
            }
        };
    }

    public static GraphQLResponse<CrmResponse> GenerateInvalidResponse()
    {
        return new GraphQLResponse<CrmResponse>()
        {
            Data = new CrmResponse
            {
                UserPerson = new List<UserPersons>(),
                PersonAccounts = new List<PersonAccount>()
            },
            Errors = new GraphQLError[]
            {
                new GraphQLError(),
                new GraphQLError()
            }
        };
    }

    public static GraphQLResponse<CrmResponse> GenerateInvalidWithMissingParamIdForAddressQueryResponse()
    {
        return new GraphQLResponse<CrmResponse>()
        {
            Data = new CrmResponse
            {
                UserPerson = new List<UserPersons>(),
                PersonAccounts = new List<PersonAccount>()
            },
            Errors = new GraphQLError[]
            {
                new GraphQLError
                {
                    Path = new ErrorPath { "userPerson", "person", "domesticAddress" }
                },
                new GraphQLError
                {
                    Path = new ErrorPath { "userPerson", "person", "internationalAddress" }
                }
            }
        };
    }

    public static GraphQLResponse<CrmResponse> GenerateEffectiveDateNullResponse()
    {
        return new GraphQLResponse<CrmResponse>()
        {
            Data = new CrmResponse
            {
                UserPerson = new List<UserPersons>()
                {
                    new()
                    {
                        Persons = new List<CrmPerson>()
                        {
                            new Bogus.Faker<CrmPerson>()
                                .RuleFor(u => u.FirstName, p => p.Person.FirstName)
                        }
                    }
                },
                PersonAccounts = new List<PersonAccount>()
                {
                    new()
                    {
                        EffectiveDate = null
                    }
                }
            }
        };
    }

    public static GraphQLResponse<CrmResponse> GeneratePersonsNullResponse()
    {
        return new GraphQLResponse<CrmResponse>()
        {
            Data = new CrmResponse
            {
                UserPerson = new List<UserPersons>()
                {
                    new()
                    {
                        Persons = null!
                    }
                },
                PersonAccounts = new List<PersonAccount>()
                {
                    new Bogus.Faker<PersonAccount>()
                        .RuleFor(u => u.EffectiveDate, f => f.Date.Past())
                }
            }
        };
    }
}
