namespace GreenLine
{
    public class SiteMenuItem : ISiteLink
    {
        public int MenuID { get; set; }
        public int ParentID { get; set; }
        public string DisplayName { get; set; }
        public string NavigationURL { get; set; }
        public bool IsActive { get; set; }
        public int DisplayRank { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Action1 { get; set; }
        public string Action2 { get; set; }
        public string Action3 { get; set; }
        public int MenuLevel { get; set; }
        public bool IsNewPortal { get; set; }

        public bool IsFavorite { get; set; }
    }
}