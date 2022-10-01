#if NETCOREAPP
using Umbraco.Extensions;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Services;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Core.Models.ContentEditing;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Cache;
#else
using System.Net;
using System.Web.Mvc;
using System.Net.Http;
using Umbraco.Web.WebApi;
using Umbraco.Core.Cache;
using Umbraco.Core.Services;
using Umbraco.Core.Logging;
using Umbraco.Core.Mapping;
using Umbraco.Core.Models.Membership;
using Umbraco.Web.Models.ContentEditing;
#endif
using System;
using uSupport.Dtos;
using uSupport.Dtos.Tables;
using uSupport.Migrations.Schemas;
using uSupport.Services.Interfaces;
using static uSupport.Helpers.uSupportTypeHelper;
using static uSupport.Constants.uSupportConstants;

namespace uSupport.Controllers
{
	public class uSupportTicketAuthorizedApiController : UmbracoAuthorizedApiController
	{
		private readonly IUserService _userService;
		private readonly AppCaches _appCaches;
#if NETCOREAPP
        private readonly IUmbracoMapper _umbracoMapper;
        private readonly ILogger<IuSupportTicketService> _logger;
#else
		private readonly ILogger _logger;
		private readonly UmbracoMapper _umbracoMapper;
#endif
        private readonly IuSupportTicketService _uSupportTicketService;
		private readonly IuSupportSettingsService _uSupportSettingsService;
		private readonly IuSupportTicketStatusService _uSupportTicketStatusService;
		private readonly IuSupportTicketCommentService _uSupportTicketCommentService;

		public uSupportTicketAuthorizedApiController(IUserService userService,
			AppCaches appCaches,
#if NETCOREAPP
            IUmbracoMapper umbracoMapper,
            ILogger<IuSupportTicketService> logger,
#else
			UmbracoMapper umbracoMapper,
			ILogger logger,
#endif
            IuSupportTicketService uSupportTicketService,
			IuSupportTicketStatusService uSupportTicketStatusService,
			IuSupportSettingsService uSupportSettingsService,
			IuSupportTicketCommentService uSupportTicketCommentService)
		{
            _logger = logger;
			_appCaches = appCaches;
			_umbracoMapper = umbracoMapper;
            _userService = userService;
			_uSupportTicketService = uSupportTicketService;
			_uSupportTicketStatusService = uSupportTicketStatusService;
			_uSupportSettingsService = uSupportSettingsService;
			_uSupportTicketCommentService = uSupportTicketCommentService;
		}

		[HttpGet]
		public uSupportPage<uSupportTicket> GetPagedActiveTickets(long page)
		{
			return _appCaches.RuntimeCache.GetCacheItem(uSupportActivePagedTicketCacheKey + page, () =>
			{
				return _uSupportTicketService.GetPagedActiveTickets(page);
			});
		} 

		[HttpGet]
		public uSupportPage<uSupportTicket> GetPagedResolvedTickets(long page)
		{
            return _appCaches.RuntimeCache.GetCacheItem(uSupportResolvedPagedTicketCacheKey + page, () =>
            {
                return _uSupportTicketService.GetPagedResolvedTickets(page);
            });
        }
			

		[HttpGet]
		public bool AnyResolvedTickets() => _uSupportTicketService.AnyResolvedTickets();

		[HttpGet]
		public uSupportTicket GetTicket(Guid ticketId)
		{
			var ticket = _uSupportTicketService.Get(ticketId);
			var author = _userService.GetUserById(ticket.AuthorId);

            ticket.Author = _umbracoMapper.Map<IUser, UserDisplay>(author);
            ticket.Comments = _uSupportTicketCommentService.GetCommentsFromTicketId(ticketId);

			return ticket;
        } 

#if NETCOREAPP
		[HttpPost]
		public ActionResult<uSupportTicket> CreateTicket(uSupportTicketSchema ticket)
		{
			try
			{
				ticket.StatusId = _uSupportTicketStatusService.GetDefaultStatus().Id;
				var createdTicket = _uSupportTicketService.Create(ticket);

                var author = _userService.GetUserById(ticket.AuthorId);
                createdTicket.Author = _umbracoMapper.Map<IUser, UserDisplay>(author);

				_uSupportSettingsService.SendEmail(
					_uSupportSettingsService.GetTicketUpdateEmailSetting(),
					_uSupportSettingsService.GetEmailSubjectNewTicket(),
					_uSupportSettingsService.GetEmailTemplateNewTicketPath(),
					createdTicket);

				_uSupportTicketService.ClearTicketCache();

				return createdTicket;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to create ticket");
				return ValidationProblem("Failed to create ticket");
			}
		}

		[HttpPost]
		public ActionResult<uSupportTicket> UpdateTicket(UpdateTicketDto dto)
		{
            try
            {
				var oldTicket = _uSupportTicketService.Get(dto.Ticket.Id);

				var oldStatus = _uSupportTicketStatusService.Get(oldTicket.StatusId);
				if (oldStatus.Id != dto.Ticket.StatusId)
				{
					var newStatus = _uSupportTicketStatusService.Get(dto.Ticket.StatusId);
					if (newStatus.Active)
						dto.Ticket.Resolved = default;
					else
						dto.Ticket.Resolved = DateTime.Now;
				}

				if (oldTicket.TypeId != dto.Ticket.TypeId)
					dto.Ticket.PropertyValue = null;

				var updatedTicket = _uSupportTicketService.Update(dto.Ticket.ConvertDtoToSchema());

				if (dto.SendEmail)
				{
					_uSupportSettingsService.SendEmail(
						_userService.GetUserById(dto.Ticket.AuthorId).Email,
						_uSupportSettingsService.GetEmailSubjectUpdateTicket(),
						_uSupportSettingsService.GetEmailTemplateUpdateTicketPath(),
						updatedTicket);
				}

                _uSupportTicketService.ClearTicketCache();

                return updatedTicket;
			}
            catch (Exception ex)
            {
				_logger.LogError(ex, "Failed to update ticket '{TicketId}'", dto.Ticket.ExternalTicketId);
				return ValidationProblem("Failed to update ticket");
			}
		}

#else
		[HttpPost]
		public HttpResponseMessage CreateTicket(uSupportTicketSchema ticket)
		{
			try
			{
				ticket.StatusId = _uSupportTicketStatusService.GetDefaultStatus().Id;
				var createdTicket = _uSupportTicketService.Create(ticket);

                var author = _userService.GetUserById(ticket.AuthorId);
                createdTicket.Author = _umbracoMapper.Map<IUser, UserDisplay>(author);

				_uSupportSettingsService.SendEmail(
					_uSupportSettingsService.GetTicketUpdateEmailSetting(),
					_uSupportSettingsService.GetEmailSubjectNewTicket(),
					_uSupportSettingsService.GetEmailTemplateNewTicketPath(),
					createdTicket);

				_uSupportTicketService.ClearTicketCache();

				return Request.CreateResponse(HttpStatusCode.OK, createdTicket.Id);
			}
			catch (Exception ex)
			{
				_logger.Error<IuSupportTicketService>(ex, ex.Message, ex.Data);

				return Request.CreateValidationErrorResponse();
			}
		}

		[HttpPost]
		public HttpResponseMessage UpdateTicket(UpdateTicketDto dto)
		{
			try
			{
				var oldTicket = _uSupportTicketService.Get(dto.Ticket.Id);

				var oldStatus = _uSupportTicketStatusService.Get(oldTicket.StatusId);
				if (oldStatus.Id != dto.Ticket.StatusId)
				{
					var newStatus = _uSupportTicketStatusService.Get(dto.Ticket.StatusId);
					if (!newStatus.Active)
						dto.Ticket.Resolved = DateTime.Now;
				}

				if (oldTicket.TypeId != dto.Ticket.TypeId)
					dto.Ticket.PropertyValue = null;

				var updatedTicket = _uSupportTicketService.Update(dto.Ticket.ConvertDtoToSchema());

				if (dto.SendEmail)
				{
					var author = _userService.GetUserById(updatedTicket.AuthorId);

					updatedTicket.Author = _umbracoMapper.Map<IUser, UserDisplay>(author);
					updatedTicket.Comments = _uSupportTicketCommentService.GetCommentsFromTicketId(updatedTicket.Id);

					_uSupportSettingsService.SendEmail(
						_userService.GetUserById(dto.Ticket.AuthorId).Email,
						_uSupportSettingsService.GetEmailSubjectUpdateTicket(),
						_uSupportSettingsService.GetEmailTemplateUpdateTicketPath(),
						updatedTicket);
				}

				_uSupportTicketService.ClearTicketCache();

				return Request.CreateResponse(HttpStatusCode.OK, updatedTicket.Id);
			}
			catch (Exception ex)
			{
				_logger.Error<IuSupportTicketService>(ex, "Failed to update ticket '{TicketId}'", dto.Ticket.ExternalTicketId);
                return Request.CreateValidationErrorResponse();
            }
		}
#endif

		[HttpGet]
		public void DeleteTicket(Guid ticketId)
		{
			_uSupportTicketService.Delete(ticketId);
            _uSupportTicketService.ClearTicketCache();
        }
	}
}