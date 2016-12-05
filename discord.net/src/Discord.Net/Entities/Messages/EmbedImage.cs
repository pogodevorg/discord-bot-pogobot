using Model = Discord.API.EmbedImage;

namespace Discord
{
    public struct EmbedImage
    {
        public string Url { get; }
        public string ProxyUrl { get; }
        public int? Height { get; }
        public int? Width { get; }

        public EmbedImage(string url, string proxyUrl, int? height, int? width)
        {
            Url = url;
            ProxyUrl = proxyUrl;
            Height = height;
            Width = width;
        }
        internal static EmbedImage Create(Model model)
        {
            return new EmbedImage(model.Url, model.ProxyUrl,
                  model.Height.IsSpecified ? model.Height.Value : (int?)null,
                  model.Width.IsSpecified ? model.Width.Value : (int?)null);
        }
        internal EmbedImage(Model model)
            : this(model.Url, model.ProxyUrl, (int?)model.Height, (int?)model.Width) { }
    }
}