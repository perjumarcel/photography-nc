using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Photography.Roles.Dto;
using Photography.Users.Dto;

namespace Photography.Users
{
    public interface IUserAppService : IAsyncCrudAppService<UserDto, long, PagedResultRequestDto, CreateUserDto, UpdateUserDto>
    {
        Task<ListResultDto<RoleDto>> GetRoles();
    }
}