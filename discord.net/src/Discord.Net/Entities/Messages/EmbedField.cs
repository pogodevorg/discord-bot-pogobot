using Model = Discord.API.EmbedField;

namespace Discord
{
    public struct EmbedField
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public bool Inline { get; set; }

        public EmbedField(int index, string name, string value, bool inline)
        {
            Index = index;
            Name = name;
            Value = value;
            Inline = inline;
        }
        internal static EmbedField Create(Model model)
        {
            return new EmbedField(model.Index, model.Name, model.Value, model.Inline);
        }
        internal EmbedField(Model model)
            : this(model.Index, model.Name, model.Value, model.Inline) { }
    }
}
