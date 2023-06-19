using System.Threading.Tasks;

namespace uSupport.Services.Interfaces
{
	public interface IuSupportSettingsService
	{
		void SendEmail(string toAddress, string subject, string templateViewPath, object model);
		bool GetSendEmailOnTicketCreatedSetting();
        string GetTicketUpdateEmailSetting();
		string GetEmailSubjectNewTicket();
		string GetEmailSubjectUpdateTicket();
		string GetEmailTemplateNewTicketPath();
		string GetEmailTemplateUpdateTicketPath();
	}
}