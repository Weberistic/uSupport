using uSupport.Constants;

namespace uSupport.Dtos.Settings
{
	public class uSupportSettingsTicket
	{
		public const string Tickets = uSupportConstants.SectionAlias + ":Settings:Tickets";
		public string TicketUpdateEmail { get; set; } = "None";
		public string EmailSubjectNewTicket { get; set; } = "A new ticket has been created";
		public string EmailSubjectUpdateTicket { get; set; } = "Your ticket has been updated";
		public string EmailTemplateNewTicketPath { get; set; } = "/App_Plugins/uSupport/templates/NewTicketEmail.cshtml";
		public string EmailTemplateUpdateTicketPath { get; set; } = "/App_Plugins/uSupport/templates/UpdateTicketEmail.cshtml";
	}
}