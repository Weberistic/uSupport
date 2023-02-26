#if NETCOREAPP
using System;
using Umbraco.Cms.Core;
using Umbraco.Extensions;
using Umbraco.Cms.Core.Trees;
using Umbraco.Cms.Core.Events;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Actions;
using Microsoft.AspNetCore.Http;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Models.Trees;
using Umbraco.Cms.Web.BackOffice.Trees;
using Umbraco.Cms.Web.Common.Attributes;
using Microsoft.AspNetCore.Authorization;
using Umbraco.Cms.Web.Common.Authorization;
#else
using Umbraco.Core;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;
using Umbraco.Web.Actions;
using Umbraco.Web.Models.Trees;
using System.Net.Http.Formatting;
using Umbraco.Web.WebApi.Filters;
using static Umbraco.Core.Constants.Trees;
using static Umbraco.Core.Constants.System;
#endif
using uSupport.Constants;
using uSupport.Dtos.Tables;
using uSupport.Services.Interfaces;


namespace uSupport.Backoffice.Trees
{
    [PluginController(uSupportConstants.SectionAlias)]
#if NETCOREAPP
	[Authorize(Policy = AuthorizationPolicies.TreeAccessDocumentTypes)]
#else
    [UmbracoTreeAuthorize(DocumentTypes)]
#endif
    [Tree(uSupportConstants.SectionAlias, uSupportConstants.TicketsTreeAlias, SortOrder = 0, TreeTitle = "Tickets", TreeGroup = uSupportConstants.TreeGroupAlias)]
    public class uSupportTicketsTreeController : TreeController
    {

        private readonly IuSupportTicketService _uSupportTicketService;
#if NETCOREAPP
        private readonly IMenuItemCollectionFactory _menuItemCollectionFactory;

        public uSupportTicketsTreeController(
            IuSupportTicketService uSupportTicketService,
            ILocalizedTextService localizedTextService,
            UmbracoApiControllerTypeCollection umbracoApiControllerTypeCollection,
            IMenuItemCollectionFactory menuItemCollectionFactory,
            IEventAggregator eventAggregator)
            : base(localizedTextService, umbracoApiControllerTypeCollection, eventAggregator)
        {
            _uSupportTicketService = uSupportTicketService;
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
            root.RoutePath = string.Format("/{0}/{1}/{2}", uSupportConstants.SectionAlias, "tickets", "overview");
            root.Icon = "icon-inbox-full";
            root.HasChildren = false;
            root.MenuUrl = null;

            return root;
        }

        protected override ActionResult<TreeNodeCollection> GetTreeNodes(string id, FormCollection queryStrings)
        {
            var nodes = new TreeNodeCollection();

            if (id == Umbraco.Cms.Core.Constants.System.Root.ToInvariantString())
            {
                foreach (uSupportTicket ticket in _uSupportTicketService.GetAll())
                {
                    var node = CreateTreeNode($"{ticket.Id}", "tickets", queryStrings, ticket.Title);
                    node.AdditionalData.Add("overviewRoutePath", "uSupport/tickets/overview");
                    node.AdditionalData.Add("type", "tickets");
                    nodes.Add(node);
                }
            }

            return nodes;
        }

        protected override ActionResult<MenuItemCollection> GetMenuForNode(string id, FormCollection queryStrings)
        {
            var menu = _menuItemCollectionFactory.Create();
            if (id != Umbraco.Cms.Core.Constants.System.Root.ToInvariantString())
            {
                menu.Items.Add<ActionDelete>(LocalizedTextService, true, true).LaunchDialogView("/App_Plugins/uSupport/components/actions/delete.html", "Delete");
            }

            return menu;
        }
#else
        public uSupportTicketsTreeController(IuSupportTicketService uSupportTicketService)
        {
            _uSupportTicketService = uSupportTicketService;
        }

        protected override TreeNode CreateRootNode(FormDataCollection queryStrings)
        {
            var root = base.CreateRootNode(queryStrings);
            root.RoutePath = string.Format("/{0}/{1}/{2}", uSupportConstants.SectionAlias, "tickets", "overview");
            root.Icon = "icon-inbox-full";
            root.HasChildren = false;
            root.MenuUrl = null;

            return root;
        }

        protected override MenuItemCollection GetMenuForNode(string id, [ModelBinder(typeof(HttpQueryStringModelBinder))] FormDataCollection queryStrings)
        {
            var menu = new MenuItemCollection();
            if (id != Root.ToInvariantString())
            {
                menu.Items.Add<ActionDelete>(Services.TextService, true).LaunchDialogView("/App_Plugins/uSupport/components/actions/delete.html", "Delete");
            }

            return menu;
        }

        protected override TreeNodeCollection GetTreeNodes(string id, [ModelBinder(typeof(HttpQueryStringModelBinder))] FormDataCollection queryStrings)
        {
            var nodes = new TreeNodeCollection();

            if (id == Root.ToInvariantString())
            {
                foreach (uSupportTicket ticket in _uSupportTicketService.GetAll())
                {
                    var node = CreateTreeNode($"{ticket.Id}", "tickets", queryStrings, ticket.Title);
                    node.AdditionalData.Add("overviewRoutePath", "uSupport/tickets/overview");
                    node.AdditionalData.Add("type", "tickets");
                    nodes.Add(node);
                }
            }

            return nodes;
        }
#endif
    }
}