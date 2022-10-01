#if NETCOREAPP
using System;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Trees;
using Umbraco.Cms.Core.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.BackOffice.Trees;
using Umbraco.Cms.Web.Common.Attributes;
using Microsoft.AspNetCore.Authorization;
using Umbraco.Cms.Web.Common.Authorization;
#else
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;
using Umbraco.Web.Models.Trees;
using System.Net.Http.Formatting;
using Umbraco.Web.WebApi.Filters;
using System.Web.Http.ModelBinding;
using static Umbraco.Core.Constants.Trees;
#endif
using uSupport.Constants;
using static uSupport.Constants.uSupportConstants;

namespace uSupport.Backoffice.Trees
{
    [PluginController(uSupportConstants.SectionAlias)]
#if NETCOREAPP
	[Authorize(Policy = AuthorizationPolicies.TreeAccessDocumentTypes)]
#else
    [UmbracoTreeAuthorize(DocumentTypes)]
#endif
    [Tree(uSupportConstants.SectionAlias, "settings", SortOrder = 5, TreeTitle = "Settings", TreeGroup = TreeGroupAlias)]
    public class uSupportSettingsTreeController : TreeController
    {
#if NETCOREAPP
		private readonly IMenuItemCollectionFactory _menuItemCollectionFactory;

		public uSupportSettingsTreeController(
			ILocalizedTextService localizedTextService,
			UmbracoApiControllerTypeCollection umbracoApiControllerTypeCollection,
			IMenuItemCollectionFactory menuItemCollectionFactory,
			IEventAggregator eventAggregator)
			: base(localizedTextService, umbracoApiControllerTypeCollection, eventAggregator)
		{
			_menuItemCollectionFactory = menuItemCollectionFactory ?? throw new ArgumentNullException(nameof(menuItemCollectionFactory));
		}


		protected override ActionResult<TreeNode> CreateRootNode(FormCollection queryStrings)
		{
			var rootResult = base.CreateRootNode(queryStrings);
			if (!(rootResult.Result is null))
			{
				return rootResult;
			}

			var root = rootResult.Value;
			root.RoutePath = "uSupport/settings/overview";
			root.Icon = "icon-settings";
			root.HasChildren = false;

			root.MenuUrl = null;

			return root;
		}

		protected override ActionResult<TreeNodeCollection> GetTreeNodes(string id, FormCollection queryStrings)
		{
			var nodes = new TreeNodeCollection();

			return nodes;
		}

		protected override ActionResult<MenuItemCollection> GetMenuForNode(string id, FormCollection queryStrings)
		{
			var menu = _menuItemCollectionFactory.Create();

			return menu;
		}
#else

        protected override TreeNode CreateRootNode(FormDataCollection queryStrings)
        {
			var root = base.CreateRootNode(queryStrings);

			root.RoutePath = "uSupport/settings/overview";
			root.Icon = "icon-settings";
			root.HasChildren = false;

			root.MenuUrl = null;

			return root;
		}

        protected override MenuItemCollection GetMenuForNode(string id, [ModelBinder(typeof(HttpQueryStringModelBinder))] FormDataCollection queryStrings)
        {
			var menu = new MenuItemCollection();

			return menu;
		}

        protected override TreeNodeCollection GetTreeNodes(string id, [ModelBinder(typeof(HttpQueryStringModelBinder))] FormDataCollection queryStrings)
        {
			var nodes = new TreeNodeCollection();

			return nodes;
		}
#endif
    }
}