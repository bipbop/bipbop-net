using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using BipbopNet.Parser;
using BipbopNet.Push;

namespace BipbopNet.Juristek
{
    [Serializable]
    public class Client
    {
        /// <summary>
        ///     Estados Válidos do País
        /// </summary>
        public enum Estado
        {
            [Description("Acre")] AC,
            [Description("Alagoas")] AL,
            [Description("Amapá")] AP,
            [Description("Amazonas")] AM,
            [Description("Bahia")] BA,
            [Description("Ceará")] CE,
            [Description("Distrito Federal")] DF,
            [Description("Espirito Santo")] ES,
            [Description("Goiás")] GO,
            [Description("Maranhão")] MA,
            [Description("Mato Grosso")] MT,
            [Description("Mato Grosso do Sul")] MS,
            [Description("Minas Gerais")] MG,
            [Description("Pará")] PA,
            [Description("Paraíba")] PB,
            [Description("Paraná")] PR,
            [Description("Pernambuco")] PE,
            [Description("Piauí")] PI,
            [Description("Rio de Janeiro")] RJ,
            [Description("Rio Grande do Norte")] RN,
            [Description("Rio Grande do Sul")] RS,
            [Description("Rondônia")] RO,
            [Description("Roraima")] RR,
            [Description("Santa Catarina")] SC,
            [Description("São Paulo")] SP,
            [Description("Sergipe")] SE,
            [Description("Tocantins")] TO
        }

        private readonly Lazy<Task<Info>> _lazyDescription;
        private readonly Lazy<Push.Client> _lazyPushClient;

        /// <summary>
        ///     Cliente BIPBOP
        /// </summary>
        public readonly BipbopNet.Client BipbopClient;

        /// <summary>
        ///     Inicia Cliente Juristek
        /// </summary>
        /// <code>
        /// var JuristekClient = new Juristek.Client(new Client());
        /// </code>
        /// <param name="bipbopClient">Cliente BIPBOP</param>
        public Client(BipbopNet.Client bipbopClient = null)
        {
            BipbopClient = bipbopClient ?? new BipbopNet.Client();
            _lazyPushClient = new Lazy<Push.Client>(() => new Push.Client(BipbopClient, Manager.Juristek));
            _lazyDescription = new Lazy<Task<Info>>(RequestDescription);
        }

        /// <summary>
        ///     WebSocket do Cliente
        /// </summary>
        public WebSocket WebSocket => BipbopClient.WebSocket;

        /// <summary>
        ///     Cliente de PUSH configurado para Juristek
        /// </summary>
        public Push.Client Push => _lazyPushClient.Value;

        /// <summary>
        ///     Consultas disponíveis no modelo Jurídico
        /// </summary>
        public Task<Info> Description => _lazyDescription.Value;

        /// <summary>
        ///     Configurações da OAB
        /// </summary>
        /// <returns>Configurações da Consulta OAB</returns>
        public async Task<OabConsulta> OabProcesso(OABParameters parameters)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            return new OabConsulta(await BipbopClient.JRequest("SELECT FROM 'OABPROCESSO'.'CONSULTA'", parameters));
        }

        /// <summary>
        ///     Configuração de PUSH específica da Juristek
        /// </summary>
        /// <param name="query">Consulta</param>
        /// <param name="userConfiguration">Configuração</param>
        /// <returns>Configuração do PUSH</returns>
        public static Configuration CreatePushConfiguration(Query query, Configuration userConfiguration = null)
        {
            var configuration = userConfiguration == null
                ? new Configuration()
                : (Configuration) userConfiguration.Clone();
            var parameters = configuration.Parameters?.ToList() ?? new List<KeyValuePair<string, string>>();
            configuration.Query = "SELECT FROM 'JURISTEK'.'PUSH'";
            parameters.Add(new KeyValuePair<string, string>("data", query.ToString()));
            if (configuration.Callback != null)
                parameters.Add(new KeyValuePair<string, string>("juristekCallback", configuration.Callback.ToString()));
            configuration.Parameters = parameters;
            return configuration;
        }

        private async Task<Info> RequestDescription()
        {
            var description = await BipbopClient.Request("SELECT FROM 'JURISTEK'.'INFO'",
                new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("data", "SELECT FROM 'INFO'.'INFO'")
                });

            return new Info(description.Document);
        }

        /// <summary>
        ///     Executa uma requisição realtime na Juristek (não recomendado)
        /// </summary>
        /// <param name="query">Consulta</param>
        /// <returns>Documento BIPBOP</returns>
        public async Task<BipbopDocument> Request(Query query)
        {
            return await BipbopClient.Request("SELECT FROM 'JURISTEK'.'REALTIME'", new[]
            {
                new KeyValuePair<string, string>("data", query.ToString())
            });
        }
    }
}