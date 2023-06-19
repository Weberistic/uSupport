#if NETCOREAPP
using uSupport.Notifications;
using Umbraco.Cms.Core.Events;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Services;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Models.ContentEditing;
using Umbraco.Cms.Web.BackOffice.Controllers;
#else
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Umbraco.Web.WebApi;
using Umbraco.Core.Logging;
using Umbraco.Core.Services;
using Umbraco.Core.Mapping;
using Umbraco.Web.Models.ContentEditing;
using Umbraco.Core.Models.Membership;
#endif
using System;
using System.Linq;
using uSupport.Dtos.Tables;
using System.Collections.Generic;
using uSupport.Migrations.Schemas;
using uSupport.Services.Interfaces;

namespace uSupport.Controllers
{
    public class uSupportTicketCommentAuthorizedApiController : UmbracoAuthorizedApiController
    {
#if NETCOREAPP
        private readonly ILogger<IuSupportTicketCommentService> _logger;
        private readonly IUmbracoMapper _umbracoMapper;
        private readonly IEventAggregator _eventAggregator;
#else
		private readonly ILogger _logger;
		private readonly UmbracoMapper _umbracoMapper;
#endif
        private readonly IUserService _userService;
        private readonly IuSupportTicketService _uSupportTicketService;
        private readonly IuSupportSettingsService _uSupportSettingsService;
        private readonly IuSupportTicketCommentService _uSupportTicketCommentService;


        public uSupportTicketCommentAuthorizedApiController(
#if NETCOREAPP

            ILogger<IuSupportTicketCommentService> logger,
            IUmbracoMapper umbracoMapper,
            IEventAggregator eventAggregator,
#else
			ILogger logger,
			UmbracoMapper umbracoMapper,
#endif
            IUserService userService,
            IuSupportTicketService uSupportTicketService,
            IuSupportSettingsService uSupportSettingsService,
            IuSupportTicketCommentService uSupportTicketCommentService)
        {
#if NETCOREAPP
            _eventAggregator = eventAggregator;
#endif
            _logger = logger;
            _userService = userService;
            _umbracoMapper = umbracoMapper;
            _uSupportTicketService = uSupportTicketService;
            _uSupportSettingsService = uSupportSettingsService;
            _uSupportTicketCommentService = uSupportTicketCommentService;
        }

        [HttpGet]
        public IEnumerable<uSupportTicketComment> GetCommentsFromTicketId(Guid ticketId) => _uSupportTicketCommentService.GetCommentsFromTicketId(ticketId).ToList();


#if NETCOREAPP
        [HttpPost]
        public ActionResult<IEnumerable<uSupportTicketComment>> Comment(uSupportTicketCommentSchema ticketComment)
        {
            try
            {
                var comment = _uSupportTicketCommentService.Create(ticketComment);

                var ticket = _uSupportTicketService.Get(ticketComment.TicketId);
                var author = _userService.GetUserById(ticket.AuthorId);
                ticket.Author = _umbracoMapper.Map<IUser, UserDisplay>(author);

                _uSupportSettingsService.SendEmail(
                    _uSupportSettingsService.GetTicketUpdateEmailSetting(),
                    _uSupportSettingsService.GetEmailSubjectNewTicket(),
                    _uSupportSettingsService.GetEmailTemplateNewTicketPath(),
                    ticket);

                _eventAggregator.Publish(new AddTicketCommentNotification(ticket, comment));

                return _uSupportTicketCommentService.GetCommentsFromTicketId(ticketComment.TicketId).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                return ValidationProblem("Failed to comment. (Check logs for futher information)");
            }
        }

        [HttpPost]
        public ActionResult<uSupportTicketComment> UpdateTicketComment(uSupportTicketCommentSchema ticketComment)
        {
            try
            {
                if (ticketComment == null) return ValidationProblem("No comment was found");

                return _uSupportTicketCommentService.Update(ticketComment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.Data);
                return ValidationProblem("Failed to update comment. (Check logs for futher information)");
            }

        }
#else
		[HttpPost]
		public IEnumerable<uSupportTicketComment> Comment(uSupportTicketCommentSchema ticketComment)
		{
			try
			{
                _uSupportTicketCommentService.Create(ticketComment);

				var ticket = _uSupportTicketService.Get(ticketComment.TicketId);
				var author = _userService.GetUserById(ticket.AuthorId);
				ticket.Author = _umbracoMapper.Map<IUser, UserDisplay>(author);

				_uSupportSettingsService.SendEmail(
					_uSupportSettingsService.GetTicketUpdateEmailSetting(),
					_uSupportSettingsService.GetEmailSubjectNewTicket(),
					_uSupportSettingsService.GetEmailTemplateNewTicketPath(),
					ticket);

				return _uSupportTicketCommentService.GetCommentsFromTicketId(ticketComment.TicketId).ToList();
			}
			catch (Exception ex)
			{
				_logger.Error<IuSupportTicketCommentService>(ex, ex.Message, ex.Data);
				throw new HttpResponseException(Request.CreateValidationErrorResponse("Failed to comment."));
			}
		}

		[HttpPost]
		public HttpResponseMessage UpdateTicketComment(uSupportTicketCommentSchema ticketComment)
		{
			try
			{
				var comment = _uSupportTicketCommentService.Update(ticketComment);
				return Request.CreateResponse(HttpStatusCode.OK, comment.Id);
			}
			catch (Exception ex)
			{
				_logger.Error<IuSupportTicketCommentService>(ex, ex.Message, ex.Data);
				return Request.CreateValidationErrorResponse("Failed to update comment.");
			}
		}
#endif

        [HttpGet]
        public void DeleteTicketComment(Guid id) => _uSupportTicketCommentService.Delete(id);
    }
}