using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Configuration;
using GreenLineDataService.Models;
using GreenLineDataService;
using GreenLineDataService.Helper.Interface;
using GreenLineDataService.Helper;

namespace GreenLine
{
    public class SiteMenuManager
    {
        public readonly IMasterService MS;

        public SiteMenuManager()
        {
            MS = new MasterService();
        }
        public string DomainName = ConfigurationManager.AppSettings["DomainName"].ToString();   

      //  public string IsDomainNameRequired = ConfigurationManager.AppSettings["IsDomainNameRequired"].ToString();
        public List<ISiteLink> GetSitemMenuItems(string Userid,string BaseFinYear)
        {
            var items = new List<ISiteLink>();       
            //string DomainName = "";

            //if (IsDomainNameRequired == "true")
            //    DomainName = "/" + MS.GetGeneralMasterObject().Where(c => c.CodeType.ToUpper() == "DOMIANNAME" && c.CodeId == "1").FirstOrDefault().CodeDesc.ToUpper();

            //DataTable dataTable = ML.GetMenuWithRights(Userid, "BindMenu");
            //List<VW_GetUserMenuRights> MenuList = MS.GetMenuListWithRights().Where(c => c.UserId == Userid && c.HasAccess == true ).ToList();
            List<VW_GetUserMenuRights> MenuList = MS.GetMenuListWithRights(Userid, false, "0",BaseFinYear).Where(c => c.HasAccess == true).ToList();

            foreach (var Mitem in MenuList)
            {
                items.Add(new SiteMenuItem
                {
                    MenuID = Mitem.MenuId,
                    ParentID = Mitem.ParentID,
                    DisplayName = Mitem.DisplayName,
                    NavigationURL = DomainName + Mitem.NavigationURL,
                    //NavigationURL =  Mitem.NavigationURL,
                    MenuLevel = Mitem.MenuLevel,
                    //NavigationURL = Mitem.CYGNUS_Master_Menu.NavigationURL,
                    IsActive = false,
                    IsNewPortal = Mitem.IsNewPortal,
                    //Controller=Mitem.Controller,
                    //Action = Mitem.Action,
                    //Action1 = Mitem.Action1,
                    //Action2 = Mitem.Action2,
                    //Action3 = Mitem.Action3,
                    IsFavorite = Mitem.IsFavorite,
                    DisplayRank = Mitem.DisplayRank
                });
            }
            return items;
        }
    }
}