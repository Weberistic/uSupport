#if NETCOREAPP
using Umbraco.Cms.Core.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models.Email;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Umbraco.Cms.Core.Configuration.Models;
#else
using System.Web;
using Umbraco.Core;
using System.Web.Mvc;
using System.Net.Mail;
using System.Web.Routing;
using Umbraco.Core.Logging;
using static System.Configuration.ConfigurationManager;
#endif
#if NET6_0
using Umbraco.Cms.Core.Extensions;
using Microsoft.Extensions.Hosting;
#elif NET5_0
using Umbraco.Cms.Core.Hosting;
#endif
using System;
using System.IO;
using uSupport.Dtos.Settings;
using uSupport.Services.Interfaces;

namespace uSupport.Services
{
	public class uSupportSettingsService : IuSupportSettingsService
	{
		private readonly IEmailSender _emailSender;
		private readonly uSupportSettingsTicket _defaultSettings;

#if NETCOREAPP
		private readonly IRazorViewEngine _razorViewEngine;
		private readonly IOptions<GlobalSettings> _globalSettings;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IOptions<uSupportSettings> _uSupportSettings;
		private readonly ITempDataProvider _tempDataProvider;
        private readonly ILogger<IuSupportTicketService> _logger;
#else
		private readonly ILogger _logger;
#endif

#if NET6_0
        private readonly IHostEnvironment _hostingEnvironment;
#elif NET5_0
		private readonly IHostingEnvironment _hostingEnvironment;
#endif
		public uSupportSettingsService(IEmailSender emailSender

#if NETCOREAPP
	, ITempDataProvider tempDataProvider,
	IRazorViewEngine razorViewEngine,
    ILogger<IuSupportTicketService> logger,
#if NET6_0
       IHostEnvironment hostingEnvironment,
#elif NET5_0
		IHostingEnvironment hostingEnvironment,
#endif
	IHttpContextAccessor httpContextAccessor,
	IOptions<GlobalSettings> globalSettings,
	IOptions<uSupportSettings> uSupportSettings
#else
	, ILogger logger
#endif
            )
        {			
#if NETCOREAPP
			_tempDataProvider = tempDataProvider;
			_globalSettings = globalSettings;
			_uSupportSettings = uSupportSettings;
			_hostingEnvironment = hostingEnvironment;
			_httpContextAccessor = httpContextAccessor;
			_razorViewEngine = razorViewEngine;
#endif
			_logger = logger;
			_emailSender = emailSender;
            _defaultSettings = new uSupportSettingsTicket();
		}

		public string GetTicketUpdateEmailSetting()
		{
#if NETCOREAPP
			return _uSupportSettings.Value.Tickets.TicketUpdateEmail;
#else
			return AppSettings["TicketUpdateEmail"];
#endif
		}

		public string GetEmailSubjectNewTicket()
		{
#if NETCOREAPP
			var emailSubjectNewTicket = _uSupportSettings.Value.Tickets.EmailSubjectNewTicket;
#else
			var emailSubjectNewTicket = AppSettings["EmailSubjectNewTicket"];
#endif
			if (!string.IsNullOrWhiteSpace(emailSubjectNewTicket))
				return emailSubjectNewTicket;

			return _defaultSettings.EmailSubjectNewTicket;
		}

		public string GetEmailSubjectUpdateTicket()
		{
#if NETCOREAPP
			var emailSubjectUpdateTicket = _uSupportSettings.Value.Tickets.EmailSubjectUpdateTicket;
#else
			var emailSubjectUpdateTicket = AppSettings["EmailSubjectUpdateTicket"];
#endif
			if (!string.IsNullOrWhiteSpace(emailSubjectUpdateTicket))
				return emailSubjectUpdateTicket;

			return _defaultSettings.EmailSubjectUpdateTicket;
		}

		public string GetEmailTemplateNewTicketPath()
		{
#if NETCOREAPP
			var emailTemplateNewTicketPath = _uSupportSettings.Value.Tickets.EmailTemplateNewTicketPath;
#else
			var emailTemplateNewTicketPath = AppSettings["EmailTemplateNewTicketPath"];
#endif
			if (!string.IsNullOrWhiteSpace(emailTemplateNewTicketPath))
				return emailTemplateNewTicketPath;

			return _defaultSettings.EmailTemplateNewTicketPath;
		}

		public string GetEmailTemplateUpdateTicketPath()
		{
#if NETCOREAPP
			var emailTemplateUpdateTicketPath = _uSupportSettings.Value.Tickets.EmailTemplateUpdateTicketPath;
#else
			var emailTemplateUpdateTicketPath = AppSettings["EmailTemplateUpdateTicketPath"];
#endif
			if (!string.IsNullOrWhiteSpace(emailTemplateUpdateTicketPath))
				return emailTemplateUpdateTicketPath;

			return _defaultSettings.EmailTemplateUpdateTicketPath;
		}

#if NETCOREAPP
		public async void SendEmail(string toAddress, string subject, string templateViewPath, object model)
		{
			try
			{
                var smtpSettings = _globalSettings.Value.Smtp;

                if (string.IsNullOrEmpty(smtpSettings?.From))
                    throw new Exception("Failed to send email. Smtp from is not set in appsettings.");

                if (string.IsNullOrEmpty(toAddress) || toAddress == "None")
                    throw new Exception("Failed to send email. TicketUpdateEmail is not set in appsettings.");

                EmailMessage message = new(smtpSettings?.From,
                                            toAddress,
                                            subject,
                                            await RenderEmailTemplateAsync(templateViewPath, model),
                                            true);

                await _emailSender.SendAsync(message, emailType: "Contact");
            }
			catch (Exception ex)
			{
                _logger.LogError(ex, "Failed to send email");
            }
		}

		public async Task<string> RenderEmailTemplateAsync(string templateViewPath, object model)
		{
            if (string.IsNullOrEmpty(templateViewPath))
                throw new Exception("Failed to find email template.");

            if (!templateViewPath.EndsWith(".cshtml"))
                throw new Exception("Template file must end with '.cshtml'");

            ActionContext actionContext = new(_httpContextAccessor.HttpContext, new Microsoft.AspNetCore.Routing.RouteData(), new ActionDescriptor());
            using (StringWriter stringWriter = new())
            {
                ViewEngineResult viewResult = _razorViewEngine.GetView(templateViewPath, templateViewPath, false);

                ViewDataDictionary viewData = new(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                };
                ViewContext viewContext = new(actionContext,
                                                viewResult.View,
                                                viewData,
                                                new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                                                stringWriter,
                                                new HtmlHelperOptions());

                await viewResult.View.RenderAsync(viewContext);

                return stringWriter.ToString();
            }
        }
#else
		public void SendEmail(string toAddress, string subject, string templateViewPath, object model)
		{
			try
			{
                if (string.IsNullOrEmpty(toAddress) || toAddress == "None")
                    throw new Exception("Failed to send email. TicketUpdateEmail is not set in web.config");

                MailMessage message = new MailMessage()
                {
                    Subject = subject,
                    IsBodyHtml = true,
                    Body = RenderEmailTemplateAsync(templateViewPath, model)
                };

                message.To.Add(toAddress);

                _emailSender.SendAsync(message);
            }
			catch (Exception ex)
			{
                _logger.Error<IuSupportSettingsService>(ex, "Failed to send email");
            }
		}

		private string RenderEmailTemplateAsync(string templateViewPath, object model)
		{

			if (string.IsNullOrEmpty(templateViewPath))
                throw new Exception("Failed to find email template.");

			if (!templateViewPath.EndsWith(".cshtml"))
                throw new Exception("Template file must end with '.cshtml'");

            using (StringWriter stringWriter = new StringWriter())
            {
                var controllerContext = CreateController();
                ViewEngineResult viewEngineResult = ViewEngines.Engines.FindView(controllerContext.ControllerContext, templateViewPath, "");
                ViewDataDictionary viewData = new ViewDataDictionary(model);

                var view = viewEngineResult.View;

                ViewContext viewContext = new ViewContext(controllerContext.ControllerContext, view, viewData, new TempDataDictionary(), stringWriter);

                viewEngineResult.View.Render(viewContext, stringWriter);

                return stringWriter.ToString();
            }

        }
#endif

#if NETFRAMEWORK
		private static EmptyController CreateController()
		{
			EmptyController instance = (EmptyController)Activator.CreateInstance(typeof(EmptyController), null);

			HttpContextBase httpContext = new HttpContextWrapper(HttpContext.Current);
			RouteData routeData = new RouteData();

			routeData.Values.Add("controller", "Empty");
			instance.ControllerContext = new ControllerContext(httpContext, routeData, instance);

			return instance;
		}
#endif
    }

#if NETFRAMEWORK
	public class EmptyController : Controller { }
#endif
}