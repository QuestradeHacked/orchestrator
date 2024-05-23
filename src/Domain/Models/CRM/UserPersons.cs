using Newtonsoft.Json;

namespace Domain.Models.CRM;

public class UserPersons
{
    public UserPersons()
    {
        Persons = new List<CrmPerson>();
    }

    [JsonProperty("person")]
    public List<CrmPerson> Persons { get; set; }
}
