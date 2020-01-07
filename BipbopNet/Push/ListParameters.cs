namespace BipbopNet.Push
{
    public class ListParameters: PushIdentifier
    {
        /// <summary>
        /// Recebe apenas PUSH com a TAG
        /// </summary>
        public string Tag;
        
        /// <summary>
        /// PUSH com ID maior que
        /// </summary>
        public string LastId;
        
        /// <summary>
        /// Pula X resultados
        /// </summary>
        public int Skip = 0;
        
        /// <summary>
        /// Quantidade Máxima de Resultados
        /// </summary>
        public int Limit = 10;
        
        /// <summary>
        /// Número da versão desejada
        /// </summary>
        public int? Version;
    }
}