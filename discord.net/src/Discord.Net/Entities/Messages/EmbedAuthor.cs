using Model = Discord.API.EmbedAuthor;

namespace Discord
{
    public struct EmbedAuthor
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string IconUrl { get; set; }
        public string ProxyIconUrl { get; set; }

        public EmbedAuthor(string name, string url, string iconUrl, string proxyIconUrl)
        {
            Name = name;
            Url = url;
            IconUrl = iconUrl;
            ProxyIconUrl = proxyIconUrl;
        }
        internal EmbedAuthor(Model model)
            : this(model.Name, model.Url, model.IconUrl, model.ProxyIconUrl) { }
    }
}
