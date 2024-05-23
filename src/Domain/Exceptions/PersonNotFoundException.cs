namespace Domain.Exceptions;

public class PersonNotFoundException : BusinessException
{
    public PersonNotFoundException()
        : base("Person with that id was not found")
    {

    }
}
