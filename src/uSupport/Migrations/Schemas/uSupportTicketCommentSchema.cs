#if NETCOREAPP
using Umbraco.Cms.Core.Models.ContentEditing;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;
#else
using Umbraco.Web.Models.ContentEditing;
using Umbraco.Core.Persistence.DatabaseAnnotations;
#endif
using NPoco;
using System;
using static uSupport.Constants.uSupportConstants;

namespace uSupport.Migrations.Schemas
{
	[ExplicitColumns, TableName(TicketCommentTableAlias)]
	[PrimaryKey("Id")]
	public class uSupportTicketCommentSchema
	{
		[PrimaryKeyColumn(AutoIncrement = false)]
		[Column("Id")]
		public Guid Id { get; set; } = Guid.NewGuid();

		[Column("TicketId")]
		public Guid TicketId { get; set; }

		[Column("UserId")]
		public int UserId { get; set; }

		[ResultColumn]
		public UserDisplay User { get; set; }

		[Column("Date")]
		public DateTime Date { get; set; } = DateTime.Now;

		[Column("Comment")]
		[SpecialDbType(SpecialDbTypes.NTEXT)]
		public string Comment { get; set; }
	}
}