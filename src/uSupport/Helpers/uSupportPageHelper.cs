using System;
using uSupport.Dtos;
using uSupport.Dtos.Tables;
using System.Collections.Generic;

namespace uSupport.Helpers
{
	public static class uSupportPageHelper
	{
		public static uSupportPage<uSupportTicket> MapPageToUSupportPage(List<uSupportTicket> items, long totalItems, long currentPage, long itemsPerPage)
		{
			uSupportPage<uSupportTicket> page = new uSupportPage<uSupportTicket>()
			{
				TotalItems = totalItems,
				ItemsPerPage = itemsPerPage,
				TotalPages = (long)Math.Ceiling((decimal)totalItems / itemsPerPage),
				CurrentPage = currentPage,
				Items = items
			};

			return page;
		}
	}
}