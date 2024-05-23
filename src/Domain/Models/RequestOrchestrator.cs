using Domain.Models.CRM;
using MediatR;

namespace Domain.Models;

public interface IRequestOrchestrator : IRequest
{
    void AddCustomerInfo(CrmPerson person, PersonAccount? personAccount);
}
