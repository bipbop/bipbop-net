using System;

namespace BipbopNet.Parser
{
    public class TargetDescription
    {
        /// <summary>
        ///     Descrição do Portal de Crawling
        /// </summary>
        public readonly string Description;

        /// <summary>
        ///     Nome do Portal de Crawling (APENAS BIPBOP)
        /// </summary>
        public readonly string Label;

        /// <summary>
        ///     Identificador do Portal de Crawling
        /// </summary>
        public readonly string Name;

        /// <summary>
        ///     URL do Portal de Crawling
        /// </summary>
        public readonly Uri Url;

        protected TargetDescription(string name, string description, string url, string label)
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