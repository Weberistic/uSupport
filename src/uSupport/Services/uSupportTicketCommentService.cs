#if NETCOREAPP
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Models.ContentEditing;
#else
using Umbraco.Core.Scoping;
using Umbraco.Core.Mapping;
using Umbraco.Core.Services;
using Umbraco.Core.Models.Membership;
using Umbraco.Web.Models.ContentEditing;
#endif
using NPoco;
using System;
using System.Linq;
using uSupport.Helpers;
using uSupport.Extensions;
using uSupport.Dtos.Tables;
using System.Collections.Generic;
using uSupport.Migrations.Schemas;
using uSupport.Services.Interfaces;
using static uSupport.Constants.uSupportConstants;

namespace uSupport.Services
{
	public class uSupportTicketCommentService : uSupportServiceBase<uSupportTicketComment, uSupportTicketCommentSchema>, IuSupportTicketCommentService
	{
		private readonly IUserService _userService;
		private static IScopeProvider _scopeProvider;
#if NETCOREAPP
		private readonly IUmbracoMapper _umbracoMapper;
#else
		private readonly UmbracoMapper _umbracoMapper;
#endif


		public uSupportTicketCommentService(IUserService userService,
			IScopeProvider scopeProvider,
#if NETCOREAPP
		 IUmbracoMapper umbracoMapper
#else
		 UmbracoMapper umbracoMapper
#endif
		) : base(TicketCommentTableAlias, scopeProvider)
		{
			_userService = userService;
			_scopeProvider = scopeProvider;
			_umbracoMapper = umbracoMapper;
		}

		public override uSupportTicketComment Get(Guid id)
		{
			using (var scope = _scopeProvider.CreateScope())
			{
				var db = scope.Database;
				var sql = new Sql()
					.Select("*")
					.From(TicketCommentTableAlias)
					.Where($"Id = UPPER('{id}')");

				return scope.Database.Fetch<uSupportTicketCommentSchema>(sql).FirstOrDefault()?.ConvertSchemaToDto();
			}
		}

		public IEnumerable<uSupportTicketComment> GetCommentsFromTicketId(Guid ticketId)
		{
			using (var scope = _scopeProvider.CreateScope())
			{
				var db = scope.Database;
				var sql = new Sql()
					.Select("*")
					.From(TicketCommentTableAlias)
					.Where($"TicketId = UPPER('{ticketId}')")
					.OrderBy("Date");

				var comments = scope.Database.Query<uSupportTicketCommentSchema>(sql);

				List<uSupportTicketComment> commentDtos = new List<uSupportTicketComment>();

				foreach (var comment in comments.ToList())
				{
					var dto = comment.ConvertSchemaToDto();
					var user = _userService.GetUserById(dto.UserId);

					dto.User = _umbracoMapper.Map<IUser, UserDisplay>(user);

					commentDtos.Add(dto);
				}

				return commentDtos;
			}
		}
	}
}