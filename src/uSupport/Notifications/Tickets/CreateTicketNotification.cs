#if NETCOREAPP
using System;
using System.Linq;
using System.Text;
using uSupport.Dtos.Tables;
using Umbraco.Cms.Core.Notifications;

namespace uSupport.Notifications
{
    public class CreateTicketNotification : INotification
    {
        public uSupportTicket Ticket { get; }
        public CreateTicketNotification(uSupportTicket ticket)
        {
            Ticket = ticket;
        }
    }
}
#endif