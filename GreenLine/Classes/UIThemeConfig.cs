using System;

namespace GreenLine.Classes
{
    public class UIThemeConfig
    {
        public PageHeaderConfig PageHeader { get; set; }
        public ButtonsConfig Buttons { get; set; }
        public TableConfig Table { get; set; }
        public SwitchConfig Switch { get; set; }
        public BadgeConfig Badge { get; set; }
        public ModalConfig Modal { get; set; }

        public class PageHeaderConfig
        {
            public string BackgroundColor { get; set; }
            public string TextColor { get; set; }
            public string FontWeight { get; set; }
            public string Padding { get; set; }
            public string FontSize { get; set; }
            public string FocusGlowColor { get; set; }
        }

        public class ButtonConfig
        {
            public string CssClass { get; set; }
            public string CssClassPre { get; set; }
            public string CssClassNext { get; set; }
            public string Icon { get; set; }
            public string Label { get; set; }
            public string Tooltip { get; set; }
            public string BackgroundColor { get; set; }
            public string TextColor { get; set; }
            public string BorderColor { get; set; }
            public string RightIcon { get; set; }
            public string LeftIcon { get; set; }
            public string RightLabel { get; set; }
            public string LeftLabel { get; set; }
        }

        public class ButtonsConfig
        {
            public ButtonConfig Add { get; set; }
            public ButtonConfig Edit { get; set; }
            public ButtonConfig Delete { get; set; }
            public ButtonConfig Excel { get; set; }
            public ButtonConfig Back { get; set; }
            public ButtonConfig Cancel { get; set; }
            public ButtonConfig Submit { get; set; }
            public ButtonConfig MenuRights { get; set; }
            public ButtonConfig ReportsRights { get; set; }
            public ButtonConfig ResetPassword { get; set; }
            public ButtonConfig UnblockUser { get; set; }
            public ButtonConfig View { get; set; }
            public ButtonConfig Previous { get; set; }
            public ButtonConfig Next { get; set; }
            public ButtonConfig Approve { get; set; }
            public ButtonConfig Reject { get; set; }
            public ButtonConfig Clear { get; set; }
        }

        public class TableConfig
        {
            public string HeaderBg { get; set; }
            public string HeaderColor { get; set; }
            public string BorderColor { get; set; }
        }

        public class SwitchConfig
        {
            public string ActiveColor { get; set; }
            public string InactiveColor { get; set; }
            public string Size { get; set; }
        }

        public class BadgeItemConfig
        {
            public string CssClass { get; set; }
            public string Label { get; set; }
            public string BackgroundColor { get; set; }
            public string TextColor { get; set; }
        }

        public class BadgeConfig
        {
            public BadgeItemConfig Active { get; set; }
            public BadgeItemConfig Inactive { get; set; }
        }

        public class ModalConfig
        {
            public string ModalTitleColor { get; set; }
            public string BackgroundColor { get; set; }
            public string BorderColor { get; set; }
            public string ModalBodyColor { get; set; }
        }
    }
}
