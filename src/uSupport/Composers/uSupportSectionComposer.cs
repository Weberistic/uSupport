#if NETCOREAPP
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
#else
using Umbraco.Web;
using Umbraco.Core.Composing;
#endif
using uSupport.Backoffice.Sections;

namespace uSupport.Composers
{
#if NETCOREAPP
	public class uSupportSectionComposer : IComposer
	{
		public void Compose(IUmbracoBuilder builder)
		{
			builder.Sections().Append<uSupportSection>();
		}
	}
#else
	public class uSupportSectionComposer : IUserComposer
	{
        public void Compose(Composition composition)
        {
			composition.Sections().Append<uSupportSection>();

		}
    }
#endif
}