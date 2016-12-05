using Model = Discord.API.EmbedFooter;

namespace Discord
{
    public struct EmbedFooter
    {
        public string Text { get; set; }
        public string IconUrl { get; set; }
        public string ProxyUrl { get; set; }

        public EmbedFooter(string text, string iconUrl, string proxyUrl)
        {
            Text = text;
            IconUrl = iconUrl;
            ProxyUrl = proxyUrl;
        }
        internal EmbedFooter(Model model)
            : this(model.Text, model.IconUrl, model.ProxyIconUrl) { }
    }
}
