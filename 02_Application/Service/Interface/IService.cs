using Core.Entity.Base;
using Core.Message.Interface;

namespace Application.Service.Interface;
public interface IService<T> where T : BaseEntity
{
    Task<IList<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task CreateAsync(IMessageCommand command);
    Task EditAsync(IMessageCommand command);
    Task DeleteAsync(IMessageCommand command);
}
