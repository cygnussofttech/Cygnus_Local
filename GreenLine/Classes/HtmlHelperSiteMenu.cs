using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System;
//using GreenLine.ViewModels;
using System.IO;
using GreenLineDataService.Models;
using GreenLineDataService.Classes;
using GreenLine.Classes;
using GreenLineDataService.Helper.Interface;
using GreenLineDataService.Helper;
namespace GreenLine
{
    public static class HtmlHelperSiteMenu
    {
        public static MvcHtmlString DynamicBreadcrumb(this HtmlHelper helper)
        {
            string Breadcrumb = buildDynamicBreadcrumb(helper, 0, null, false);
            string controller = helper.ViewContext.RouteData.Values["controller"].ToString();
            string action = helper.ViewContext.RouteData.Values["action"].ToString();
            //var CMM = db.CYGNUS_Master_Menu.Where(c => (c.Action == action || c.Action1 == action || c.Action2 == action || c.Action3 == action) && c.Controller == controller);

            //if (CMM.Count() > 0)
            //{
            //    var CMMobj = db.CYGNUS_Master_Menu.First(c => (c.Action == action || c.Action1 == action || c.Action2 == action || c.Action3 == action) && c.Controller == controller);
            //    Breadcrumb = Breadcrumb + buildDynamicBreadcrumb(helper, CMMobj.MenuID, CMMobj, false);
            //}

            return MvcHtmlString.Create(Breadcrumb);
        }

        private static string buildDynamicBreadcrumb(this HtmlHelper html, int MenuId, CYGNUS_Master_Menu MenuObj, bool IsLink)
        {
            string Breadcrumb = "";

            var itemTag1 = new TagBuilder("li");
            if (MenuId == 0)
            {
                var itemTag2 = new TagBuilder("i");
                itemTag1.MergeAttribute("class", "icon-home");
                itemTag1.InnerHtml = itemTag2.ToString();
                var anchorTag = new TagBuilder("a");
                anchorTag.MergeAttribute("href", "");
                anchorTag.SetInnerText("Home");
                itemTag1.InnerHtml += anchorTag.ToString();
                var anchorTag1 = new TagBuilder("i");
                anchorTag1.MergeAttribute("class", "icon-angle-right");
                itemTag1.InnerHtml += anchorTag1.ToString();
            }
            else
            {

                var anchorTag = new TagBuilder("a");
                if (IsLink)
                    anchorTag.MergeAttribute("href", MenuObj.NavigationURL);
                else
                    anchorTag.MergeAttribute("href", "#");
                anchorTag.SetInnerText(MenuObj.DisplayName);
                itemTag1.InnerHtml = anchorTag.ToString();
                if (IsLink)
                {
                    var anchorTag1 = new TagBuilder("i");
                    anchorTag1.MergeAttribute("class", "icon-angle-right");
                    itemTag1.InnerHtml += anchorTag1.ToString();
                }
            }
            Breadcrumb = itemTag1.InnerHtml;
            return Breadcrumb.ToString();
        }

        public static MvcHtmlString SiteMenuAsUnorderedList(this HtmlHelper helper, List<ISiteLink> siteLinks, int typ, string Username)
        {

            //string controller = helper.ViewContext.RouteData.Values["controller"].ToString();
            //string action = helper.ViewContext.RouteData.Values["action"].ToString();

            // var CMM = db.CYGNUS_Master_Menu.Where(c => (c.Action == action || c.Action1 == action || c.Action2 == action || c.Action3 == action) && c.Controller == controller);

            int SelparentId = 0, SelparentId1 = 0;
            //if (CMM.ToList().Count > 0)
            //{
            //    var CMM1 = db.CYGNUS_Master_Menu.First(c => (c.Action == action || c.Action1 == action || c.Action2 == action || c.Action3 == action) && c.Controller == controller);
            //    SelparentId = CMM1.ParentID;
            //    if (SelparentId > 0)
            //    {
            //        var CMM2 = db.CYGNUS_Master_Menu.First(c => c.MenuID == SelparentId);
            //        SelparentId1 = CMM2.ParentID;
            //    }
            //    //  var CMM3 = db.CYGNUS_Master_Menu.First(c => (c.Action == action || c.Action1 == action || c.Action2 == action || c.Action3 == action) && c.Controller == controller);
            //}

            MvcHtmlString result;
            if (siteLinks == null || siteLinks.Count == 0)
                result = MvcHtmlString.Empty;
            var topLevelParentId = SiteLinkListHelper.GetTopLevelParentId(siteLinks);
            //  string abc = "XXX" + (buildMenuItems(helper, siteLinks, topLevelParentId)) + "XXX";
            //  abc = abc.Replace("XXX<ul>", "").Replace("</ul>XXX", "");
            if (typ == 1)
                result = MvcHtmlString.Create(buildMenuItems(helper, siteLinks, topLevelParentId, SelparentId, SelparentId1));
            else if (typ == 2)
                result = MvcHtmlString.Create(buildMenuItemsHoriZontal(helper, siteLinks, topLevelParentId));
            else
                result = MvcHtmlString.Create(buildMenuItemsHoriZontalMega(helper, siteLinks, topLevelParentId));
            string folderPath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/UserMenu/");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string curFile = folderPath + "UserMenu_" + Username + "_" + typ + ".txt";
            using (StreamWriter writer = new StreamWriter(curFile))
            {
                writer.WriteLine(result);
            }
            return result;
        }

        public static MvcHtmlString UIStart(this HtmlHelper helper)
        {
            return MvcHtmlString.Create("<div class=\"row\">");
        }
        public static MvcHtmlString UIStartD(this HtmlHelper helper)
        {
            return MvcHtmlString.Create("<div class=\"form-body\">");
        }

        public static MvcHtmlString UIStartDNew(this HtmlHelper helper)
        {
            return MvcHtmlString.Create("<div class=\"row gx-10 mb-5\">");
        }


        /* GST Changes Start*/
        public static MvcHtmlString GSTUIStart(this HtmlHelper helper)
        {
            return MvcHtmlString.Create("<div class=\"row clsGSTApply\">");
        }
        public static MvcHtmlString GSTUIStartNew(this HtmlHelper helper)
        {
            return MvcHtmlString.Create("<div class=\"row gx-10 mb-5 clsGSTApply\">");
        }

        public static MvcHtmlString UIEnd(this HtmlHelper helper)
        {
            return MvcHtmlString.Create("</div>");
        }
        public static MvcHtmlString Tableend(this HtmlHelper helper)
        {
            return MvcHtmlString.Create("</tr>");
        }
        public static MvcHtmlString Tablecreate(this HtmlHelper helper)
        {
            return MvcHtmlString.Create("<tr>");
        }

        private static string buildMenuItems(this HtmlHelper html, List<ISiteLink> siteLinks, int parentId, int SelparentId, int SelparentId1)
        {
            var parentTag = new TagBuilder("ul");
            if (parentId == 5000)
            {
                parentTag.MergeAttribute("class", "page-sidebar-menu ");
                parentTag.MergeAttribute("data-auto-scroll", "true ");
                parentTag.MergeAttribute("data-slide-speed", "200");
                //var itemTag2 = new TagBuilder("div");
                //itemTag2.MergeAttribute("class", "sidebar-toggler hidden-phone");
                //var itemTag1 = new TagBuilder("li");
                //itemTag1.MergeAttribute("class", "sidebar-toggler-wrapper");
                //itemTag1.InnerHtml = itemTag2.ToString();
                //parentTag.InnerHtml = itemTag1.ToString();
            }
            else
                parentTag.MergeAttribute("class", "sub-menu");


            var childSiteLinks = SiteLinkListHelper.GetChildSiteLinks(siteLinks, parentId);
            string controller = html.ViewContext.RouteData.Values["controller"].ToString();
            string action = html.ViewContext.RouteData.Values["action"].ToString();

            foreach (var siteLink in childSiteLinks)
            {
                var itemTag = new TagBuilder("li");
                var anchorTag = new TagBuilder("a");
                var anchorTag1 = new TagBuilder("i");
                var anchorTag2 = new TagBuilder("span");
                var anchorTag3 = new TagBuilder("span");
                anchorTag2.MergeAttribute("class", "title");

                if ((siteLink.Action == action && siteLink.Controller == controller) || (SelparentId == siteLink.MenuID && SelparentId != 0))
                {
                    itemTag.AddCssClass("active");
                }

                if ((SelparentId == siteLink.MenuID || SelparentId1 == siteLink.MenuID) && SelparentId != 0)
                {
                    itemTag.AddCssClass("active");
                }

                if (siteLink.Controller == controller)
                {
                    if (siteLink.Action == action || siteLink.Action1 == action || siteLink.Action2 == action || siteLink.Action3 == action)
                        itemTag.AddCssClass("active");
                }

                //if (SelparentId == 0 && SiteLinkListHelper.SiteLinkHasChildren(siteLinks, siteLink.MenuID) == false)
                //    itemTag.AddCssClass("active");

                if (SiteLinkListHelper.SiteLinkHasChildren(siteLinks, siteLink.MenuID))
                    anchorTag3.MergeAttribute("class", "arrow");

                if (siteLink.NavigationURL == null)
                {
                    anchorTag1.MergeAttribute("class", "fa fa-folder-open");
                    anchorTag.MergeAttribute("href", "javascript:;");
                    anchorTag.InnerHtml = anchorTag1.ToString();
                }
                else
                    anchorTag.MergeAttribute("href", siteLink.NavigationURL);

                anchorTag2.SetInnerText(siteLink.DisplayName);
                anchorTag.InnerHtml += anchorTag2.ToString();
                anchorTag.InnerHtml += anchorTag3.ToString();

                if (siteLink.IsActive)
                {
                    anchorTag.MergeAttribute("target", "_blank");
                }
                itemTag.InnerHtml = anchorTag.ToString();
                if (SiteLinkListHelper.SiteLinkHasChildren(siteLinks, siteLink.MenuID))
                {
                    itemTag.InnerHtml += buildMenuItems(html, siteLinks, siteLink.MenuID, SelparentId, SelparentId1);
                }
                parentTag.InnerHtml += itemTag;
            }
            return parentTag.ToString();
        }

        private static string buildMenuItemsHoriZontal(this HtmlHelper html, List<ISiteLink> siteLinks, int parentId)
        {
            var parentTag = new TagBuilder("ul");
            if (parentId == 5000)
            {
                parentTag.MergeAttribute("class", "nav navbar-nav");
            }
            else
            {
                parentTag.MergeAttribute("class", "dropdown-menu");
            }

            var childSiteLinks = SiteLinkListHelper.GetChildSiteLinks(siteLinks, parentId);
            string controller = html.ViewContext.RouteData.Values["controller"].ToString();
            string action = html.ViewContext.RouteData.Values["action"].ToString();

            foreach (var siteLink in childSiteLinks)
            {
                var itemTag = new TagBuilder("li");
                var anchorTag = new TagBuilder("a");
                //  var anchorTag1 = new TagBuilder("i");
                var anchorTag2 = new TagBuilder("span");
                var anchorTag3 = new TagBuilder("span");

                // anchorTag1.MergeAttribute("class", "icon-home");
                anchorTag2.MergeAttribute("class", "title");

                //if ((siteLink.Action == action && siteLink.Controller == controller) || (SelparentId == siteLink.MenuID && SelparentId != 0))
                //{
                //    itemTag.AddCssClass("active");                  
                //}

                //if ((SelparentId == siteLink.MenuID || SelparentId1 == siteLink.MenuID) && SelparentId != 0)
                //{
                //    itemTag.AddCssClass("active");
                //}


                //if (siteLink.Controller == controller)
                //{
                //    if (siteLink.Action == action || siteLink.Action1 == action || siteLink.Action2 == action || siteLink.Action3 == action)
                //        itemTag.AddCssClass("active");
                //}

                //if (SelparentId == 0 && SiteLinkListHelper.SiteLinkHasChildren(siteLinks, siteLink.MenuID) == false)
                //    itemTag.AddCssClass("active");

                if (SiteLinkListHelper.SiteLinkHasChildren(siteLinks, siteLink.MenuID))
                {

                    //    anchorTag3.MergeAttribute("class", "arrow");
                    anchorTag.MergeAttribute("href", "javascript:;");

                    if (siteLink.MenuLevel > 0)
                    {
                        itemTag.MergeAttribute("class", "dropdown-submenu");
                    }
                    else
                    {
                        itemTag.MergeAttribute("class", "classic-menu-dropdown");
                        anchorTag.MergeAttribute("data-hover", "dropdown");
                        anchorTag.MergeAttribute("data-close-others", "true");
                        //   anchorTag.MergeAttribute("class", "classic-menu-dropdown");
                    }
                }
                else
                {
                    anchorTag.MergeAttribute("href", siteLink.NavigationURL);
                    anchorTag.InnerHtml += "<span class='fa fa-star " + (siteLink.IsFavorite ? "checked" : "") + "' data-id='" + siteLink.MenuID + "'></span> ";
                }
                anchorTag.InnerHtml += siteLink.DisplayName;
                //anchorTag.SetInnerText(siteLink.DisplayName);

                if (siteLink.MenuLevel == 0)
                {
                    if (SiteLinkListHelper.SiteLinkHasChildren(siteLinks, siteLink.MenuID))
                    {
                        var anchorTagitemTag = new TagBuilder("i");
                        anchorTagitemTag.MergeAttribute("class", "fa fa-angle-down");
                        anchorTag.SetInnerText(siteLink.DisplayName);

                        anchorTag.InnerHtml += anchorTagitemTag.ToString();
                    }
                }

                if (siteLink.IsActive)
                {
                    anchorTag.MergeAttribute("target", "_blank");
                }
                itemTag.InnerHtml = anchorTag.ToString();
                if (SiteLinkListHelper.SiteLinkHasChildren(siteLinks, siteLink.MenuID))
                {
                    itemTag.InnerHtml += buildMenuItemsHoriZontal(html, siteLinks, siteLink.MenuID);
                }
                parentTag.InnerHtml += itemTag;
            }
            return parentTag.ToString();
        }

        private static string buildMenuItemsHoriZontalMega(this HtmlHelper html, List<ISiteLink> siteLinks, int parentId)
        {
            var parentTag = new TagBuilder("ul");
            if (parentId == 5000)
            {
                parentTag.MergeAttribute("class", "nav navbar-nav");
                //var itemTag2 = new TagBuilder("div");
                //itemTag2.MergeAttribute("class", "visible-phone visible-tablet");
                //var itemTag1 = new TagBuilder("li");
                //itemTag1.MergeAttribute("class", "visible-phone visible-tablet");
                //  itemTag1.InnerHtml = itemTag2.ToString();
                // parentTag.InnerHtml = itemTag1.ToString();
            }
            else
            {
                parentTag.MergeAttribute("class", "col-md-4 mega-menu-submenu");
            }


            var childSiteLinks = SiteLinkListHelper.GetChildSiteLinks(siteLinks, parentId);
            string controller = html.ViewContext.RouteData.Values["controller"].ToString();
            string action = html.ViewContext.RouteData.Values["action"].ToString();

            foreach (var siteLink in childSiteLinks)
            {
                var itemTag = new TagBuilder("li");
                var DivTag = new TagBuilder("div");
                var DivTag1 = new TagBuilder("div");
                var anchorTag = new TagBuilder("a");
                //  var anchorTag1 = new TagBuilder("i");
                var anchorTag2 = new TagBuilder("span");
                var anchorTag3 = new TagBuilder("span");

                // anchorTag1.MergeAttribute("class", "icon-home");
                anchorTag2.MergeAttribute("class", "title");

                if (SiteLinkListHelper.SiteLinkHasChildren(siteLinks, siteLink.MenuID))
                {

                    //    anchorTag3.MergeAttribute("class", "arrow");
                    anchorTag.MergeAttribute("href", "javascript:;");

                    if (siteLink.MenuLevel > 0)
                    {
                        //  itemTag.MergeAttribute("class", "dropdown-submenu");
                    }
                    else
                    {
                        itemTag.MergeAttribute("class", "mega-menu-dropdown");
                        anchorTag.MergeAttribute("data-hover", "dropdown");
                        anchorTag.MergeAttribute("class", "dropdown-toggle");
                        anchorTag.MergeAttribute("data-close-others", "true");
                        //   anchorTag.MergeAttribute("class", "classic-menu-dropdown");
                    }
                }
                else
                {

                    anchorTag.MergeAttribute("href", siteLink.NavigationURL);
                }
                anchorTag.SetInnerText(siteLink.DisplayName);

                if (siteLink.MenuLevel == 0)
                {
                    if (SiteLinkListHelper.SiteLinkHasChildren(siteLinks, siteLink.MenuID))
                    {
                        var anchorTagitemTag = new TagBuilder("i");
                        anchorTagitemTag.MergeAttribute("class", "fa fa-angle-down");
                        anchorTag.SetInnerText(siteLink.DisplayName);

                        anchorTag.InnerHtml += anchorTagitemTag.ToString();
                    }
                }

                itemTag.InnerHtml += anchorTag.ToString();


                if (SiteLinkListHelper.SiteLinkHasChildren(siteLinks, siteLink.MenuID))
                {
                    var ULTag = new TagBuilder("ul");
                    var LITag = new TagBuilder("li");
                    ULTag.MergeAttribute("class", "dropdown-menu");
                    DivTag.MergeAttribute("class", "mega-menu-content");
                    DivTag1.MergeAttribute("class", "row");
                    DivTag1.InnerHtml += buildMenuItemsHoriZontalMega1(html, siteLinks, siteLink.MenuID).Replace("<A1>", "").Replace("</A1>", "");
                    DivTag.InnerHtml = DivTag1.ToString();
                    LITag.InnerHtml = DivTag.ToString();
                    ULTag.InnerHtml = LITag.ToString();
                    itemTag.InnerHtml += ULTag.ToString();
                }
                parentTag.InnerHtml += itemTag;
            }
            return parentTag.ToString();
        }

        private static string buildMenuItemsHoriZontalMega1(this HtmlHelper html, List<ISiteLink> siteLinks, int parentId)
        {
            var parentTag = new TagBuilder("A1");

            var childSiteLinks = SiteLinkListHelper.GetChildSiteLinks(siteLinks, parentId);
            string controller = html.ViewContext.RouteData.Values["controller"].ToString();
            string action = html.ViewContext.RouteData.Values["action"].ToString();

            foreach (var siteLink in childSiteLinks)
            {
                var ULTag = new TagBuilder("ul");
                ULTag.MergeAttribute("class", "col-md-4 mega-menu-submenu");
                var itemTag = new TagBuilder("li");
                var anchorTag = new TagBuilder("h3");

                anchorTag.SetInnerText(siteLink.DisplayName);

                itemTag.InnerHtml += anchorTag.ToString();

                ULTag.InnerHtml += itemTag.ToString();

                var childSiteLinks1 = SiteLinkListHelper.GetChildSiteLinks(siteLinks, siteLink.MenuID);

                foreach (var siteLink1 in childSiteLinks1)
                {
                    var itemTag1 = new TagBuilder("li");
                    var anchorTag1 = new TagBuilder("a");
                    anchorTag1.MergeAttribute("href", siteLink1.NavigationURL);

                    var anchorTagitemTag = new TagBuilder("i");
                    anchorTagitemTag.MergeAttribute("class", "fa fa-angle-right");
                    anchorTag1.InnerHtml += anchorTagitemTag.ToString();
                    anchorTag1.InnerHtml += siteLink1.DisplayName.ToString();

                    itemTag1.InnerHtml += anchorTag1.ToString();
                    ULTag.InnerHtml += itemTag1;
                }

                parentTag.InnerHtml += ULTag;

            }
            return parentTag.ToString();
        }

        private static string folderPath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/");

        public static MvcHtmlString MenuTree(this HtmlHelper helper, string UserId)
        {
           IMasterService MS = new MasterService();
            var siteLinks = new List<ISiteLink>();
            GeneralFunctions GF = new GeneralFunctions();

            //    string SQRY = " EXEC Usp_CYGNUS_GetUserMenuRights '" + UserId + "','MenuRightsModule'";
            //    DataTable dataTable = GF.GetDateTableFromQuery(SQRY);

            List<VW_GetUserMenuRights> MenuList = MS.GetMenuListWithRights(UserId, true, "1","").ToList();
            foreach (var Mitem in MenuList.ToList())
            {
                siteLinks.Add(new SiteMenuItem
                {
                    MenuID = Mitem.MenuId,
                    ParentID = Mitem.ParentID,
                    DisplayName = Mitem.DisplayName,
                    IsActive = Convert.ToBoolean(Mitem.HasAccess),
                    DisplayRank = Mitem.DisplayRank,
                    IsNewPortal = Mitem.IsNewPortal
                });
            }

            if (siteLinks == null || siteLinks.Count == 0)
                return MvcHtmlString.Empty;
            var topLevelParentId = SiteLinkListHelper.GetTopLevelParentId(siteLinks);
            string abc = MenuTreeMenuItems(siteLinks, topLevelParentId);

            return MvcHtmlString.Create(abc);
        }

        private static string MenuTreeMenuItems(List<ISiteLink> siteLinks, int parentId)
        {
            var parentTag = new TagBuilder("ul");
            var childSiteLinks = SiteLinkListHelper.GetChildSiteLinks(siteLinks, parentId);
            foreach (var siteLink in childSiteLinks)
            {
                var itemTag = new TagBuilder("li");
                itemTag.MergeAttribute("id", siteLink.MenuID.ToString() + "XXX" + siteLink.IsActive.ToString());

                itemTag.SetInnerText(siteLink.DisplayName);
                if (SiteLinkListHelper.SiteLinkHasChildren(siteLinks, siteLink.MenuID))
                {
                    itemTag.InnerHtml += MenuTreeMenuItems(siteLinks, siteLink.MenuID);
                }
                parentTag.InnerHtml += itemTag;
            }
            return parentTag.ToString();
        }
    }
}