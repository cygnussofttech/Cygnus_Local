namespace GreenLine
{
    public interface ISiteLink
    {
        int MenuID { get; set; }
        int ParentID { get; set; }
        string DisplayName { get; set; }
        string NavigationURL { get; set; }
        bool IsActive { get; set; }
        int DisplayRank { get; set; }
        string Action { get; set; }
        string Action1 { get; set; }
        string Action2 { get; set; }
        string Action3 { get; set; }
        string Controller { get; set; }
        int MenuLevel { get; set; }
        bool IsFavorite { get; set; }
    }
}