namespace BipbopNet.Juristek
{
    public class TipoInscricao
    {
        public readonly string Value;

        private TipoInscricao(string value)
        {
            Value = value;
        }

        public static TipoInscricao Advogado => new TipoInscricao("D");
        public static TipoInscricao Suplementar => new TipoInscricao("S");
        public static TipoInscricao Transferencia => new TipoInscricao("T");
        public static TipoInscricao Estagiario => new TipoInscricao("E");
        public static TipoInscricao Desconhecido => new TipoInscricao("A");
        public static TipoInscricao Normal => new TipoInscricao("N");


        public override string ToString()
        {
            return Value;
        }
    }
}