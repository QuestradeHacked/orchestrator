namespace Domain.Exceptions;

public class ProjectIdIsEmptyException : BusinessException
{
    public ProjectIdIsEmptyException()
        : base("Configuration ProjectId is empty")
    {

    }
}
