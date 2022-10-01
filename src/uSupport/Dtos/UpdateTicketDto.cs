using uSupport.Dtos.Tables;

namespace uSupport.Dtos
{
    public class UpdateTicketDto
    {
        public uSupportTicket Ticket { get; set; }
        public bool SendEmail { get; set; }
    }
}
