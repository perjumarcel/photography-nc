using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Photography.MultiTenancy.Dto;

namespace Photography.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}
