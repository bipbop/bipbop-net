namespace BipbopNet.Juristek
{

    /// <summary>
    /// Tipos de Inscrição
    /// </summary>
    public class TipoInscricao
    {
        public readonly string Value;

        private TipoInscricao(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Advogado
        /// </summary>
        public static TipoInscricao Advogado => new TipoInscricao("D");
        /// <summary>
        /// Suplementar
        /// </summary>
        public static TipoInscricao Suplementar => new TipoInscricao("S");
        /// <summary>
        /// Transferência
        /// </summary>
        public static TipoInscricao Transferencia => new TipoInscricao("T");
        /// <summary>
        /// Estagiário
        /// </summary>
        public static TipoInscricao Estagiario => new TipoInscricao("E");
        /// <summary>
        /// Desconhecido
        /// </summary>
        public static TipoInscricao Desconhecido => new TipoInscricao("A");
        /// <summary>
        /// Pessoas sem curso de direito
        /// </summary>
        public static TipoInscricao Normal => new TipoInscricao("N");


        public override string ToString()
        {
            return Value;
        }
    }
}