#if NETCOREAPP
using Umbraco.Cms.Infrastructure.Migrations;
#else
using Umbraco.Core.Migrations;
#endif
using uSupport.Migrations.Schemas;

namespace uSupport.Migrations.Create
{
    public class uSupportTicketTable : uSupportMigrationBase
    {
		public uSupportTicketTable(IMigrationContext context) : base(context)
		{ }

		protected override void DoMigrate()
		{
			Create.Table<uSupportTicketSchema>().Do();
		}
	}
}