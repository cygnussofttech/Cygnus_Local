using System.Collections.Generic;
using System.Linq;

namespace GreenLine
{
    public static class SiteLinkListHelper
    {
        public static int GetTopLevelParentId(IEnumerable<ISiteLink> siteLinks)
        {
            return siteLinks.OrderBy(i => i.ParentID).Select(i => i.ParentID).FirstOrDefault();
        }

        public static bool SiteLinkHasChildren(IEnumerable<ISiteLink> siteLinks, int id)
        {
            return siteLinks.Any(i => i.ParentID == id);
        }

        public static IEnumerable<ISiteLink> GetChildSiteLinks(IEnumerable<ISiteLink> siteLinks,
            int parentIdForChildren)
        {
            return siteLinks.Where(i => i.ParentID == parentIdForChildren)
                .OrderBy(i => i.DisplayRank).ThenBy(i => i.DisplayName);
        }
    }
}