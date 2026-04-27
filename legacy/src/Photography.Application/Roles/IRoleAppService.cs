using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Photography.Roles.Dto;

namespace Photography.Roles
{
    public interface IRoleAppService : IAsyncCrudAppService<RoleDto, int, PagedResultRequestDto, CreateRoleDto, RoleDto>
    {
        Task<ListResultDto<PermissionDto>> GetAllPermissions();
    }
}
