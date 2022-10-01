using System;
using uSupport.Dtos.Tables;
using System.Collections.Generic;
using uSupport.Migrations.Schemas;

namespace uSupport.Services.Interfaces
{
	public interface IuSupportTicketStatusService
	{
		IEnumerable<uSupportTicketStatus> GetAll();
		IEnumerable<uSupportTicketStatus> GetResolvedStatuses();
		IEnumerable<uSupportTicketStatus> GetActiveStatuses();
		uSupportTicketStatus GetDefaultStatus();
		uSupportTicketStatus Get(Guid id);
		IEnumerable<uSupportTicketStatus> GetByIds(List<Guid> ids);
		Guid GetStatusIdFromName(string name);
		uSupportTicketStatus Create(uSupportTicketStatusSchema ticketStatus);
		uSupportTicketStatus Update(uSupportTicketStatusSchema ticketStatus);
		void Delete(Guid id);
		int GetStatusCount();
	}
}