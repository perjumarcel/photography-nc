using Abp.Web.Mvc.Views;

namespace Photography.Web.Views
{
    public abstract class PhotographyWebViewPageBase : PhotographyWebViewPageBase<dynamic>
    {

    }

    public abstract class PhotographyWebViewPageBase<TModel> : AbpWebViewPage<TModel>
    {
        protected PhotographyWebViewPageBase()
        {
            LocalizationSourceName = PhotographyConsts.LocalizationSourceName;
        }
    }
}