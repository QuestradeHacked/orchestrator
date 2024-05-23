using Newtonsoft.Json;

namespace Domain.Models.CRM;

public class CrmResponse
{
    public IList<PersonAccount>? PersonAccounts { get; set; }

    [JsonProperty("userPerson")]
    public IList<UserPersons>? UserPerson { get; set; }

    public CrmResponse()
    {
        UserPerson = new List<UserPersons>();
        PersonAccounts = new List<PersonAccount>();
    }
}
