namespace MyNaati.Ui.ViewModels.Home
{
    public class MenuLinkModel
    {
        public string Text { get; set; }
        public string UrlAction { get; set; }
        public LinkIconType IconType { get; set; }
        public string Icon { get; set; }
        public string AltImageText { get; set; }
		public int Index { get; set; }
        public string Id { get; set; }


        public MenuLinkModel(string text, string urlAction, LinkIconType iconType, string icon, string altImageText = null, int index = 0, string id = null)
        {
            Text = text;
            UrlAction = urlAction;
            IconType = iconType;
            Icon = icon;
            AltImageText = altImageText;
			Index = index;
            Id = id;
        }

        public enum LinkIconType
        {
            FontAwesome,
            Image
        }
    }
}