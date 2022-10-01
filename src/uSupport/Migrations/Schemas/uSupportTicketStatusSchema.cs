using NPoco;
using static uSupport.Constants.uSupportConstants;

namespace uSupport.Migrations.Schemas
{
	[ExplicitColumns, TableName(TicketStatusTableAlias)]
	public class uSupportTicketStatusSchema : uSupportTypeBaseSchema
	{
		[Column("Default")]
		public bool Default { get; set; }

		[Column("Active")]
		public bool Active { get; set; }
	}
}