namespace Domain.Exceptions;

public class PersonalPhoneNumberIsMissingException : BusinessException
{
    public PersonalPhoneNumberIsMissingException() : base("Must have at least one personal number")
    {

    }
}
