using System;
using uSupport.Dtos;
using uSupport.Dtos.Tables;
using System.Collections.Generic;
using uSupport.Migrations.Schemas;

namespace uSupport.Services.Interfaces
{
	public interface IuSupportTicketService
	{
		IEnumerable<uSupportTicket> GetAll();
		uSupportPage<uSupportTicket> GetPagedResolvedTickets(long page);
		uSupportPage<uSupportTicket> GetPagedActiveTickets(long page);
		bool AnyResolvedTickets();
		uSupportTicket Get(Guid id);
		uSupportTicket Create(uSupportTicketSchema ticket);
		uSupportTicket Update(uSupportTicketSchema ticketDto);
		void Delete(Guid id);
		void ClearTicketCache();
	}
}