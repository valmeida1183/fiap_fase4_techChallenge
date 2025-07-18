using Application.Service.Interface;
using Core.Entity.Base;
using Core.Message.Interface;
using Core.Repository.Interface;

namespace Application.Service.Base;
public abstract class BaseService<T> : IService<T> where T : BaseEntity
{
    protected readonly IHttpRepository<T> _repository;
    protected readonly IMessagePublisher _messagePublisher;

    protected BaseService(IHttpRepository<T> repository, IMessagePublisher messagePublisher)
    {
        _repository = repository;
        _messagePublisher = messagePublisher;
    }

    public virtual async Task<IList<T>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }
}
