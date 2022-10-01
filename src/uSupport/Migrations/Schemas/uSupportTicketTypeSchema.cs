#if NETCOREAPP
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;
#else
using Umbraco.Core.Persistence.DatabaseAnnotations;
#endif
using NPoco;
using static uSupport.Constants.uSupportConstants;

namespace uSupport.Migrations.Schemas
{
	[ExplicitColumns, TableName(TicketTypeTableAlias)]
	public class uSupportTicketTypeSchema : uSupportTypeBaseSchema
	{
		[Column("Description")]
        [NullSetting]
        public string Description { get; set; }

        [Column("PropertyId")]
        [NullSetting]
        public int PropertyId { get; set; }

        [Column("PropertyName")]
        [NullSetting]
        public string PropertyName { get; set; }

        [Column("PropertyDescription")]
        [NullSetting]
        public string PropertyDescription { get; set; }

        [Column("PropertyView")]
        [NullSetting]
        public string PropertyView { get; set; }
    }
}