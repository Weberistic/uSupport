using System;
using uSupport.Dtos.Tables;
using System.Collections.Generic;
using uSupport.Migrations.Schemas;

namespace uSupport.Services.Interfaces
{
	public interface IuSupportTicketCommentService
	{
		IEnumerable<uSupportTicketComment> GetCommentsFromTicketId(Guid ticketId);
		uSupportTicketComment Get(Guid id);
		uSupportTicketComment Create(uSupportTicketCommentSchema comment);
		uSupportTicketComment Update(uSupportTicketCommentSchema comment);
		void Delete(Guid id);
	}
}