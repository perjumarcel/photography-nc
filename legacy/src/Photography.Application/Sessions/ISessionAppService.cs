using System.Threading.Tasks;
using Abp.Application.Services;
using Photography.Sessions.Dto;

namespace Photography.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
