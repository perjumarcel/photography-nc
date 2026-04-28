using System.Threading.Tasks;
using Abp.Application.Services;
using Photography.Configuration.Dto;

namespace Photography.Configuration
{
    public interface IConfigurationAppService: IApplicationService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}