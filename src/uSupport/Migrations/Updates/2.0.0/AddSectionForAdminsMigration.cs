#if NETCOREAPP
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Cms.Infrastructure.Migrations;
using static Umbraco.Cms.Core.Constants;
#else
using Umbraco.Core;
using Umbraco.Web;
using Umbraco.Core.Scoping;
using Umbraco.Core.Services;
using Umbraco.Core.Migrations;
using static Umbraco.Core.Constants;
#endif

namespace uSupport.Migrations.Updates._2._0._0
{
	public class AddSectionForAdminsMigration : uSupportMigrationBase
	{
		private readonly IUserService _userService;
		private readonly IScopeProvider _scopeProvider;
		private readonly IUmbracoContextFactory _umbracoContext;

		public AddSectionForAdminsMigration(IUserService userService,
			IUmbracoContextFactory umbracoContext,
			IMigrationContext context) : base(context)
		{
			_userService = userService;
			_umbracoContext = umbracoContext;
		}

		protected override void DoMigrate()
		{
			using (UmbracoContextReference umbracoContextReference = _umbracoContext.EnsureUmbracoContext())
			{
				using (var scope = _scopeProvider.CreateScope())
				{
					var adminGroup = _userService.GetUserGroupByAlias(Security.AdminGroupAlias);
					adminGroup.AddAllowedSection(Constants.uSupportConstants.SectionAlias);

					_userService.Save(adminGroup);

					scope.Complete();
				}
			}
		}
	}
}