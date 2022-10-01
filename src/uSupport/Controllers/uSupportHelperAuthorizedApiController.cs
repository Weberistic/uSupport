#if NETCOREAPP
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Mapping;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Models.ContentEditing;
using Umbraco.Cms.Web.BackOffice.Controllers;
#else
using System.Web.Http;
using Umbraco.Web.WebApi;
using Umbraco.Core.Models;
using Umbraco.Core.Mapping;
using Umbraco.Core.Services;
using Umbraco.Core.Models.Membership;
using Umbraco.Web.Models.ContentEditing;
#endif
using System;
using System.Linq;
using uSupport.Dtos;
using Newtonsoft.Json;
using System.Collections.Generic;


namespace uSupport.Controllers
{
	public class uSupportHelperAuthorizedApiController : UmbracoAuthorizedApiController
	{
		private readonly IUserService _userService;
#if NETCOREAPP
		private readonly IUmbracoMapper _umbracoMapper;
#else
		private readonly UmbracoMapper _umbracoMapper;
#endif
		private readonly IEntityService _entityService;

		public uSupportHelperAuthorizedApiController(IUserService userService,
			#if NETCOREAPP
		IUmbracoMapper umbracoMapper,
#else
		UmbracoMapper umbracoMapper,
#endif
		IEntityService entityService)
		{
			_userService = userService;
			_umbracoMapper = umbracoMapper;
			_entityService = entityService;
		}

		[HttpGet]
		public string GenerateExternalTicketId() => $"{GenerateRandomLetters(3)}-{GenerateRandomLetters(4)}-{GenerateRandomNumbers(3)}";

		[HttpGet]
		public UserDisplay GetUserById(int id)
		{
			var user = _userService.GetUserById(id);
			var result = _umbracoMapper.Map<IUser, UserDisplay>(user);
			return result;
		}

		[HttpPost]
		public UserDisplay SaveUser(uSupportUserSaveDto uSupportUserSaveDto)
		{
			var user = _userService.GetUserById(uSupportUserSaveDto.Id);

			user.Name = uSupportUserSaveDto.Name;
			user.Email = uSupportUserSaveDto.Email;

			_userService.Save(user);

			return GetUserById(user.Id);
		}

		[HttpPost]
		public IUser ClearAvatar(UserDisplay displayUser)
		{
			var user = _userService.GetUserById(int.Parse(displayUser.Id.ToString()));
			user.Avatar = "";

			_userService.Save(user);

			return user;
		}

		[HttpPost]
		public object ReadOnlyValue(object value)
		{
			if (value != null)
			{
				switch (Type.GetTypeCode(value.GetType()))
				{
					case TypeCode.Object:
						return JsonConvert.DeserializeObject<IEnumerable<uSupportReadonlyDto>>($"{value}");

					default:
						int id = int.Parse($"{value}");
						UmbracoObjectTypes objectType = _entityService.GetObjectType(id);
						return new List<object> ()
						{
							_entityService.Get(id, objectType)
						};
				}
			}

			return value;
		}

		private string GenerateRandomNumbers(int length)
		{
			var sequence = Enumerable.Range(1, 9).OrderBy(n => n * n * (new Random()).Next());

			return string.Join("", sequence.Distinct().Take(length));
		}

		private string GenerateRandomLetters(int length)
		{
			Random random = new Random();
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
		}
	}
}