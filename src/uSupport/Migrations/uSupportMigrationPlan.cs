#if NETCOREAPP
using Umbraco.Cms.Infrastructure.Migrations;
#else
using Umbraco.Core.Migrations;
#endif
using uSupport.Migrations.Create;

namespace uSupport.Migrations
{
	public class uSupportMigrationPlan : MigrationPlan
	{
		public uSupportMigrationPlan() : base("uSupport")
		{
			DefinePlan();
		}

		protected void DefinePlan()
		{
			From(string.Empty)
				.To<uSupportTicketCommentTable>("uSupport-ticket-comment")
				.To<uSupportTicketTypeTable>("uSupport-ticket-type")
				.To<uSupportTicketStatusTable>("uSupport-ticket-status")
				.To<uSupportTicketTable>("uSupport-ticket");
		}
	}
}