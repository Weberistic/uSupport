using uSupport.Constants;

namespace uSupport.Dtos.Settings
{
	public class uSupportSettings
	{
		public const string uSupport = uSupportConstants.SectionAlias + ":Settings";

		public uSupportSettingsTicket Tickets { get; set; } = new uSupportSettingsTicket();
	}
}