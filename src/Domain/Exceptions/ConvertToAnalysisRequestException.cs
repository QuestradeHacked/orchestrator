namespace Domain.Exceptions;

public class ConvertToAnalysisRequestException : BusinessException
{
    public ConvertToAnalysisRequestException()
        : base("Error converting to AnalysisRequest."){}
}
