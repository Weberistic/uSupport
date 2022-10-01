#if NETCOREAPP
using Umbraco.Cms.Core.Models.ContentEditing;
#else
using Umbraco.Web.Models.ContentEditing;
#endif
using NPoco;
using System;
using System.Collections.Generic;

namespace uSupport.Dtos.Tables
{
	public class uSupportTicket
	{
        public Guid Id { get; set; }
		public string Title { get; set; }
		public string Summary { get; set; }
		public Guid TypeId { get; set; }
		public uSupportTicketType Type { get; set; }
		public Guid StatusId { get; set; }
		public uSupportTicketStatus Status{ get; set; }
		public int AuthorId { get; set; }

		[ResultColumn]
		public UserDisplay Author { get; set; }
        public DateTime Submitted { get; set; }
		public DateTime? Resolved { get; set; }
		public string LastUpdatedBy { get; set; }
		public string ExternalTicketId { get; set; }
		public string PropertyValue { get; set; }
		public IEnumerable<uSupportTicketComment> Comments { get; set; }
	}
}