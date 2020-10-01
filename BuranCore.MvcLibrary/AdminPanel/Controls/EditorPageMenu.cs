using System.Collections.Generic;

namespace Buran.Core.MvcLibrary.AdminPanel.Controls
{
    public enum EditPageMenuItemType
    {
        Other,
        Insert,
        Back,
        Save,
        SaveContinue,
        SaveNew,
        Edit,
        Delete
    }

    public class EditorPageMenu
    {
        public List<EditorPageMenuItem> Items;

        public EditorPageMenu()
        {
            Items = new List<EditorPageMenuItem>();
        }
    }

    public class EditorPageMenuSubItem
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public string ButtonClass { get; set; }
        public string Target { get; set; }
    }

    public class EditorPageMenuSplitItem
    {
        public string Id { get; set; }
        public string Url { get; set; }

        public string Title { get; set; }
        public string Target { get; set; }

        public string ButtonClass { get; set; }
    }

    public class EditorPageMenuItem
    {
        public string Id { get; set; }
        public string Url { get; set; }

        public string Title { get; set; }
        public string IconClass { get; set; }
        public string ButtonClass { get; set; }
        public string ButtonIdClass { get; set; }
        public string Target { get; set; }
        public EditPageMenuItemType ItemType { get; set; }


        public string PostUrl { get; set; }
        public string ConfirmText { get; set; }

        public List<EditorPageMenuSubItem> Items;
        public List<EditorPageMenuSplitItem> SplitItems;

        public EditorPageMenuItem()
        {
            Items = new List<EditorPageMenuSubItem>();
            SplitItems = new List<EditorPageMenuSplitItem>();
        }
    }
}
