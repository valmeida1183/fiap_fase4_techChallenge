using Core.Entity;
using Core.Message.Command;

namespace Application.Service.Interface;
public interface IContactService : IService<Contact>
{
    Task<IList<Contact>> GetAllByDddAsync(int dddId);
    Task<string> ResilienceTest(bool fail);

    Task CreateMessageAsync(CreateContactCommand command);
}
