namespace BipbopNet
{
    public class TargetDescription
    {
        public readonly string? Description;
        public readonly string? Name;
        public readonly string? Url;

        protected TargetDescription(string? name, string? description, string? url)
        {
            Name = name;
            Description = description;
            Url = url;
        }
    }
}