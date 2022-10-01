#if NETCOREAPP
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Dashboards;
#else
using Umbraco.Core.Composing;
using Umbraco.Core.Dashboards;
#endif
using System;
using uSupport.Constants;

namespace uSupport.Backoffice.Dashboards
{
	[Weight(-10)]
	public class AdminDashboard : IDashboard
	{
		public string Alias => "uSupportAdminDashboard";
		public string[] Sections => new[]
		{
			uSupportConstants.SectionAlias
		};
		public IAccessRule[] AccessRules => Array.Empty<IAccessRule>();
		public string View => "/App_Plugins/uSupport/components/dashboards/admin/dashboard.html";
	}
}