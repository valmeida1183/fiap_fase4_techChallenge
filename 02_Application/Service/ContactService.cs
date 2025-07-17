using Application.Service.Base;
using Application.Service.Interface;
using Core.Entity;
using Core.Message.Command;
using Core.Message.Interface;
using Core.Repository.Interface;

namespace Application.Service;
public class ContactService : BaseService<Contact>, IContactService
{
    private readonly IContactHttpRepository _contactHttpRepository;
    private readonly IDirectDistanceDialingHttpRepository _directDistanceDialingHttpRepository;

    public ContactService(IContactHttpRepository contactHttpRepository, IDirectDistanceDialingHttpRepository directDistanceDialingHttpRepository, IMessagePublisher messagePublisher) : base(contactHttpRepository, messagePublisher)
    {
        _contactHttpRepository = contactHttpRepository;
        _directDistanceDialingHttpRepository = directDistanceDialingHttpRepository; 
    }


    public async Task<IList<Contact>> GetAllByDddAsync(int dddId)
    {
        var ddd = await _directDistanceDialingHttpRepository.GetByIdAsync(dddId);

        return ddd is null
            ? throw new ArgumentException("Invalid Direct Distance Dialing Id")
            : await _contactHttpRepository.GetAllByDddAsync(dddId);
    }

    public async Task<string> ResilienceTest(bool fail)
    {
        return await _contactHttpRepository.ResilienceTest(fail);
    }
    public async Task CreateMessageAsync(CreateContactCommand command)
    {
        await _messagePublisher.Publish(command);
    }
}
