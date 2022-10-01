#if NETCOREAPP
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Web.BackOffice.Controllers;
#else
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Umbraco.Web.WebApi;
using Umbraco.Core.Logging;
#endif
using System;
using uSupport.Helpers;
using uSupport.Dtos.Tables;
using System.Collections.Generic;
using uSupport.Migrations.Schemas;
using uSupport.Services.Interfaces;

namespace uSupport.Controllers
{
	public class uSupportTicketStatusAuthorizedApiController : UmbracoAuthorizedApiController
	{
#if NETCOREAPP
		private readonly ILogger<IuSupportTicketStatusService> _logger;
#else
		private readonly ILogger _logger;
#endif
		private readonly IuSupportTicketStatusService _uSupportTicketStatusService;

		public uSupportTicketStatusAuthorizedApiController(
#if NETCOREAPP
			ILogger<IuSupportTicketStatusService> logger,
#else
			ILogger logger,
#endif
			IuSupportTicketStatusService uSupportTicketStatusService)
		{
			_logger = logger;
			_uSupportTicketStatusService = uSupportTicketStatusService;
		}

		[HttpGet]
		public IEnumerable<uSupportTicketStatus> GetAllTicketStatuses() => _uSupportTicketStatusService.GetAll();

		[HttpGet]
		public uSupportTicketStatus GetTicketStatus(Guid id) => _uSupportTicketStatusService.Get(id);

		[HttpPost]
		public IEnumerable<uSupportTicketStatus> GetTicketStatuses(List<Guid> ids)
        {
			if (ids == null) return null;
			
			return _uSupportTicketStatusService.GetByIds(ids);
		}

		[HttpGet]
		public Guid GetStatusIdFromName(string statusName) => _uSupportTicketStatusService.GetStatusIdFromName(statusName);

#if NETCOREAPP
		[HttpPost]
		public ActionResult<uSupportTicketStatus> CreateTicketStatus(uSupportTicketStatusSchema ticketStatus)
		{
            try 
			{
				ticketStatus.Order = _uSupportTicketStatusService.GetStatusCount() + 1;

				return _uSupportTicketStatusService.Create(ticketStatus);
			}
            catch (Exception ex)
            {
				_logger.LogError(ex, "Failed to create ticket status.");
				return ValidationProblem("Failed to create ticket status.");
            }
		}

		[HttpPost]
		public ActionResult<uSupportTicketStatus> UpdateTicketStatus(uSupportTicketStatus ticketStatus)
        {
            try
            {
				return _uSupportTicketStatusService.Update(ticketStatus.ConvertDtoToSchema());
			}
            catch (Exception ex)
            {
				_logger.LogError(ex, "Failed to update ticket status '{TicketStatusName}'", ticketStatus.Name);
				return ValidationProblem("Failed to update ticket status.");
			}
			
		}
#else
        [HttpPost]
		public HttpResponseMessage CreateTicketStatus(uSupportTicketStatusSchema ticketStatus)
		{
			try
			{
				ticketStatus.Order = _uSupportTicketStatusService.GetStatusCount() + 1;

				var createdTicketStatus = _uSupportTicketStatusService.Create(ticketStatus);

				return Request.CreateResponse(HttpStatusCode.OK, createdTicketStatus.Id);
			}
			catch (Exception ex)
			{
				_logger.Error<IuSupportTicketStatusService>(ex, "Failed to create ticket status.");
				return Request.CreateValidationErrorResponse("Failed to create ticket status.");
			}
		}

		[HttpPost]
		public HttpResponseMessage UpdateTicketStatus(uSupportTicketStatus ticketStatus)
        {
            try
            {
				var updatedTicketStatus = _uSupportTicketStatusService.Update(ticketStatus.ConvertDtoToSchema());

				return Request.CreateResponse(HttpStatusCode.OK, updatedTicketStatus.Id);
			}
			catch (Exception ex)
			{
				_logger.Error<IuSupportTicketStatusService>(ex, "Failed to update ticket status '{TicketStatusId}'.", ticketStatus.Name);
				return Request.CreateValidationErrorResponse("Failed to update ticket status");
			}
		}
#endif

		[HttpGet]
		public void DeleteTicket(Guid id) => _uSupportTicketStatusService.Delete(id);
	}
}