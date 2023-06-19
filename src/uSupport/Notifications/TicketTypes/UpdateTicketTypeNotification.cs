#if NETCOREAPP
using uSupport.Dtos.Tables;
using Umbraco.Cms.Core.Notifications;

namespace uSupport.Notifications
{
    public class UpdateTicketTypeNotification : INotification
    {
        public uSupportTicket Ticket { get; }
        public UpdateTicketStatusNotification(uSupportTicket ticket)
        {
            Ticket = ticket;
        }
    }
}
#endif