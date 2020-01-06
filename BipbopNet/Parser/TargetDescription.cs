using System;

namespace BipbopNet.Parser
{
    public class TargetDescription
    {
        public readonly string? Description;
        public readonly string? Name;
        public readonly Uri? Url;
        public readonly string? Label;

        protected TargetDescription(string? name, string? description, string? url, string? label)
        {
            Name = name?.Replace("'", "");
            Description = description;
            Label = label;
            Url = string.IsNullOrEmpty(url) ? null : new Uri(url);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}