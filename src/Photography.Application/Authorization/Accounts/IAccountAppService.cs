using System.Threading.Tasks;
using Abp.Application.Services;
using Photography.Authorization.Accounts.Dto;

namespace Photography.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
