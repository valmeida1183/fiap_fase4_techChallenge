using Application.Service.Base;
using Application.Service.Interface;
using Core.Entity;
using Core.Message.Interface;
using Core.Repository.Interface;

namespace Application.Service;
public class DirectDistanceDialingService : BaseService<DirectDistanceDialing>, IDirectDistanceDialingService
{
    public DirectDistanceDialingService(IDirectDistanceDialingHttpRepository repository) : base(repository)
    {        
    }
}
