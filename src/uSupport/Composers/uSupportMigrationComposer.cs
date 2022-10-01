#if NETCOREAPP
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.DependencyInjection;
#else
using Umbraco.Core;
using Umbraco.Core.Scoping;
using Umbraco.Core.Logging;
using Umbraco.Core.Services;
using Umbraco.Core.Composing;
using Umbraco.Core.Migrations;
using Umbraco.Core.Migrations.Upgrade;
#endif
using uSupport.Migrations;

namespace uSupport.Composers
{
#if NETCOREAPP
	public class uSupportMigrationComposer : IComposer
	{
		public void Compose(IUmbracoBuilder builder)
		{
			builder.AddNotificationHandler<UmbracoApplicationStartingNotification, RunuSupportMigration>();
		}
	}
#else
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class uSupportMigrationComposer : ComponentComposer<uSupportMigrationComponent> { }

    public class uSupportMigrationComponent : IComponent
	{
		private readonly ILogger _logger;
		private readonly IScopeProvider _scopeProvider;
		private readonly IKeyValueService _keyValueService;
		private readonly IMigrationBuilder _migrationPlanExecutor;

		public uSupportMigrationComponent(ILogger logger,
			IScopeProvider scopeProvider,
			IKeyValueService keyValueService,
			IMigrationBuilder migrationPlanExecutor)
        {
			_logger = logger;
			_scopeProvider = scopeProvider;
			_keyValueService = keyValueService;
			_migrationPlanExecutor = migrationPlanExecutor;
        }

		public void Initialize()
        {
            var uppgrader = new Upgrader(new uSupportMigrationPlan());
            uppgrader.Execute(_scopeProvider, _migrationPlanExecutor, _keyValueService, _logger);
        }

        public void Terminate() { }
    }
#endif
}