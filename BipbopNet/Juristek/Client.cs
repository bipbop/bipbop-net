using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using BipbopNet.Parser;
using BipbopNet.Push;

namespace BipbopNet.Juristek
{
    public class Client
    {
        /// <summary>
        /// Estados Válidos do País
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
        /// Cliente BIPBOP
        /// </summary>
        public readonly BipbopNet.Client BipbopClient;

        /// <summary>
        /// Inicia Cliente Juristek
        /// </summary>
        /// <code>
        /// var JuristekClient = new Juristek.Client(new Client());
        /// </code>
        /// <param name="bipbopClient">Cliente BIPBOP</param>
        public Client(BipbopNet.Client bipbopClient)
        {
            BipbopClient = bipbopClient;
            _lazyPushClient = new Lazy<Push.Client>(() => new Push.Client(BipbopClient, Manager.Juristek));
            _lazyDescription = new Lazy<Task<Info>>(RequestDescription);
        }

        /// <summary>
        /// Cliente de PUSH configurado para Juristek
        /// </summary>
        public Push.Client Push => _lazyPushClient.Value;
        
        /// <summary>
        /// Consultas disponíveis no modelo Jurídico
        /// </summary>
        public Task<Info> Description => _lazyDescription.Value;

        /// <summary>
        /// Consulta de Processo OAB
        /// </summary>
        /// <param name="numeroOab">
        /// Número da OAB completo
        /// <example>12345-PE</example>
        /// </param>
        /// <param name="pushCallback">Callback da Requisição</param>
        /// <param name="userEstado">Estado da Pesquisa</param>
        /// <param name="tipoInscricao">Tipo de Inscrição</param>
        /// <returns></returns>
        public async Task<OabConsulta> OabProcesso(string numeroOab, Uri pushCallback, Estado? userEstado = null,
            TipoInscricao tipoInscricao = null)
        {
            return new OabConsulta(await BipbopClient.JRequest("SELECT FROM 'OABPROCESSO'.'CONSULTA'", new[]
            {
                new KeyValuePair<string, string>("numero_oab", numeroOab),
                new KeyValuePair<string, string>("pushCallback", pushCallback.ToString()),
                new KeyValuePair<string, string>("estado", userEstado == null ? null : userEstado.ToString()),
                new KeyValuePair<string, string>("tipoInscricao",
                    tipoInscricao == null ? TipoInscricao.Advogado.ToString() : tipoInscricao.ToString())
            }));
        }

        /// <summary>
        /// Configuração de PUSH específica da Juristek
        /// </summary>
        /// <param name="query">Consulta</param>
        /// <param name="userConfiguration">Configuração</param>
        /// <returns>Configuração do PUSH</returns>
        public static PushConfiguration CreatePushConfiguration(Query query, PushConfiguration userConfiguration = null)
        {
            var configuration = userConfiguration == null
                ? new PushConfiguration()
                : (PushConfiguration) userConfiguration.Clone();
            var parameters = configuration.Parameters?.ToList() ?? new List<KeyValuePair<string, string>>();
            configuration.Query = "SELECT FROM 'JURISTEK'.'PUSH'";
            parameters.Add(new KeyValuePair<string, string>("data", query.ToString()));
            if (configuration.Callback != null)
                parameters.Add(new KeyValuePair<string, string>("juristekCallback", configuration.Callback));
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
        /// Executa uma requisição realtime na Juristek (não recomendado)
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