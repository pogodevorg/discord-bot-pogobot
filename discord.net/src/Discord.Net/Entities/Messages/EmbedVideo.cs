using Model = Discord.API.EmbedVideo;

namespace Discord
{
    public struct EmbedVideo
    {
        public string Url { get; }
        public int? Height { get; }
        public int? Width { get; }

        public EmbedVideo(string url, int? height, int? width)
        {
            Url = url;
            Height = height;
            Width = width;
        }
        internal static EmbedVideo Create(Model model)
        {
            return new EmbedVideo(model.Url,
                  model.Height.IsSpecified ? model.Height.Value : (int?)null,
                  model.Width.IsSpecified ? model.Width.Value : (int?)null);
        }
        internal EmbedVideo(Model model)
            : this(model.Url, (int?)model.Height, (int?)model.Width) { }
    }
}