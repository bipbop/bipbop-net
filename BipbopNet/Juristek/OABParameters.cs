using System;
using System.Collections.Generic;

namespace BipbopNet.Juristek
{
    /// <summary>
    ///     Configuração de uma Consulta da OAB
    /// </summary>
    [Serializable]
    public class OABParameters : List<KeyValuePair<string, string>>
    {
        /// <summary>
        ///     Configuração da OAB
        /// </summary>
        /// <param name="numeroOab">Numeração da consulta OAB</param>
        public OABParameters(string numeroOab)
        {
            if (string.IsNullOrWhiteSpace(numeroOab))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(numeroOab));
            Add(new KeyValuePair<string, string>("NumeroOab", numeroOab));
        }

        /// <summary>
        ///     Estado de Origem da OAB (caso o estado seja confuso na numeração)
        /// </summary>
        public Client.Estado OrigemOab
        {
            set => Add(new KeyValuePair<string, string>("OrigemOab", value.ToString()));
        }


        /// <summary>
        ///     Configura Recebimento pelo WebSocket
        /// </summary>
        public bool WebSocket
        {
            set => Add(new KeyValuePair<string, string>("WebSocket", value ? "true" : "false"));
        }

        /// <summary>
        ///     Configura o CALLBACK das consultas
        /// </summary>
        public Uri Callback
        {
            set => Add(new KeyValuePair<string, string>("PushCallback", value.ToString()));
        }

        /// <summary>
        ///     Define uma TAG universal para todas as consultas
        /// </summary>
        public string Marker
        {
            get => Find(item => item.Key == "Marker").Value;
            set => Add(new KeyValuePair<string, string>("Marker", value));
        }

        /// <summary>
        ///     Define uma label que diferencia as consultas
        /// </summary>
        public string Label
        {
            set => Add(new KeyValuePair<string, string>("Label", value));
        }

        /// <summary>
        ///     Estado onde a consulta será realizada
        /// </summary>
        public Client.Estado Estado
        {
            set => Add(new KeyValuePair<string, string>("Estado", value.ToString()));
        }

        /// <summary>
        ///     Tipo da Inscrição
        /// </summary>
        public TipoInscricao TipoInscricao
        {
            set => Add(new KeyValuePair<string, string>("TipoInscricao", value.ToString()));
        }

        /// <summary>
        ///     Configura uma versão máxima para as consultas (0 => infinito)
        /// </summary>
        public int MaxVersion
        {
            set => Add(new KeyValuePair<string, string>("MaxVersion", value.ToString()));
        }
    }
}