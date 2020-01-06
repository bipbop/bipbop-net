using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using BipbopNet.Parser;
using BipbopNet.Push;

namespace BipbopNet.Juristek
{
    public class Client
    {
        
        public enum Estado
        {
            [Description("Acre")]
            AC,
            [Description("Alagoas")]
            AL,
            [Description("Amapá")]
            AP,
            [Description("Amazonas")]
            AM,
            [Description("Bahia")]
            BA,
            [Description("Ceará")]
            CE,
            [Description("Distrito Federal")]
            DF,
            [Description("Espirito Santo")]
            ES,
            [Description("Goiás")]
            GO,
            [Description("Maranhão")]
            MA,
            [Description("Mato Grosso")]
            MT,
            [Description("Mato Grosso do Sul")]
            MS,
            [Description("Minas Gerais")]
            MG,
            [Description("Pará")]
            PA,
            [Description("Paraíba")]
            PB,
            [Description("Paraná")]
            PR,
            [Description("Pernambuco")]
            PE,
            [Description("Piauí")]
            PI,
            [Description("Rio de Janeiro")]
            RJ,
            [Description("Rio Grande do Norte")]
            RN,
            [Description("Rio Grande do Sul")]
            RS,
            [Description("Rondônia")]
            RO,
            [Description("Roraima")]
            RR,
            [Description("Santa Catarina")]
            SC,
            [Description("São Paulo")]
            SP,
            [Description("Sergipe")]
            SE,
            [Description("Tocantins")]
            TO
        }
        public readonly BipbopNet.Client BipbopClient;
        
        public Push.Client Push => _lazyPushClient.Value;
        public Task<Info> Description => _lazyDescription.Value;

        public Client(BipbopNet.Client bipbopClient)
        {
            BipbopClient = bipbopClient;
            _lazyPushClient = new Lazy<Push.Client>(() => new Push.Client(BipbopClient, Manager.Juristek));
            _lazyDescription = new Lazy<Task<Info>>(RequestDescription);
        }


        private readonly Lazy<Task<Info>> _lazyDescription;
        private readonly Lazy<Push.Client> _lazyPushClient;

        public async Task<OabConsulta> OABProcesso(string numeroOab, Uri pushCallback, Estado? userEstado = null, TipoInscricao? tipoInscricao = null)
        {
            return new OabConsulta(await BipbopClient.JRequest("SELECT FROM 'OABPROCESSO'.'CONSULTA'", new []
            {
                KeyValuePair.Create("numero_oab", numeroOab),
                KeyValuePair.Create("pushCallback", pushCallback.ToString()),
                KeyValuePair.Create("estado", userEstado == null ? null : userEstado.ToString()),
                KeyValuePair.Create("tipoInscricao", tipoInscricao == null ? TipoInscricao.Advogado.ToString() : tipoInscricao.ToString()),
            }));
         
        }

        public PushConfiguration CreatePushConfiguration(Query query, PushConfiguration? userConfiguration = null)
        {
            var configuration = userConfiguration == null ? new PushConfiguration() : (PushConfiguration) userConfiguration.Clone();
            configuration.Parameters ??= new List<KeyValuePair<string, string>>();
            configuration.Query = "SELECT FROM 'JURISTEK'.'PUSH'";
            configuration.Parameters.Add(KeyValuePair.Create("data", query.ToString()));
            if (configuration.Callback != null) configuration.Parameters.Add(KeyValuePair.Create("juristekCallback", configuration.Callback));
            return configuration;
        }
        
        private async Task<Info> RequestDescription()
        {
            var description = await BipbopClient.Request("SELECT FROM 'JURISTEK'.'INFO'", new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("data", "SELECT FROM 'INFO'.'INFO'"),
            });

            return new Info(description.Document);
        }

        public async Task<BipbopDocument> Request(Query query)
        {
            return await BipbopClient.Request("SELECT FROM 'JURISTEK'.'REALTIME'", new[]
            {
                new KeyValuePair<string, string>("data", query.ToString()),
            });
        }
    }
}