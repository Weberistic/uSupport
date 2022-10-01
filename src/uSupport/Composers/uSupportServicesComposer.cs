#if NETCOREAPP
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
#else
using Umbraco.Core;
using Umbraco.Core.Composing;
#endif
using uSupport.Services;
using uSupport.Services.Interfaces;

namespace uSupport.Composers
{
#if NETCOREAPP
	public class uSupportServicesComposer : IComposer
	{
		public void Compose(IUmbracoBuilder builder)
		{
			builder.Services.AddScoped<IuSupportTicketTypeService, uSupportTicketTypeService>()
							.AddScoped<IuSupportTicketStatusService, uSupportTicketStatusService>()
							.AddScoped<IuSupportTicketCommentService, uSupportTicketCommentService>()
							.AddScoped<IuSupportTicketService, uSupportTicketService>()
							.AddScoped<IuSupportSettingsService, uSupportSettingsService>();
		}
	}
#else
	[RuntimeLevel(MinLevel = RuntimeLevel.Run)]
	public class uSupportServicesComposer : IUserComposer
	{
        public void Compose(Composition composition)
        {
			composition.Register<IuSupportTicketTypeService, uSupportTicketTypeService>(Lifetime.Request);
			composition.Register<IuSupportTicketStatusService, uSupportTicketStatusService>(Lifetime.Request);
			composition.Register<IuSupportTicketCommentService, uSupportTicketCommentService>(Lifetime.Request);
			composition.Register<IuSupportTicketService, uSupportTicketService>(Lifetime.Request);
			composition.Register<IuSupportSettingsService, uSupportSettingsService>(Lifetime.Request);
		}
    }
#endif
}