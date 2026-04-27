using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using Photography.Configuration.Dto;

namespace Photography.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : PhotographyAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
