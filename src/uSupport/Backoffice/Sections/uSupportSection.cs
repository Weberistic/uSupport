#if NETCOREAPP
using Umbraco.Cms.Core.Sections;
#else
using Umbraco.Core.Models.Sections;
#endif

namespace uSupport.Backoffice.Sections
{
	public class uSupportSection : ISection
	{
		public string Alias => "uSupport";
		public string Name => "Support";
	}
}