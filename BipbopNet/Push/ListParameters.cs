using System;

namespace BipbopNet.Push
{
    [Serializable]
    public class ListParameters : JobIdentifier
    {
        /// <summary>
        ///     PUSH com ID maior que
        /// </summary>
        public string LastId;

        /// <summary>
        ///     Quantidade Máxima de Resultados
        /// </summary>
        public int Limit = 10;

        /// <summary>
        ///     Pula X resultados
        /// </summary>
        public int Skip = 0;

        /// <summary>
        ///     Recebe apenas PUSH com a TAG
        /// </summary>
        public string Tag;

        /// <summary>
        ///     Número da versão desejada
        /// </summary>
        public int? Version;
    }
}