using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using BipbopNet.Parser;

namespace BipbopNet.Push
{
    /// <summary>
    ///     Agendamento de Trabalho BIPBOP
    /// </summary>
    /// <code>
    /// var client = new Client();
    /// var pushClient = new Push.Client(new Client());
    /// </code>
    [Serializable]
    public class Client
    {
        /// <summary>
        ///     Cliente da API
        /// </summary>
        public readonly BipbopNet.Client BipbopClient;

        /// <summary>
        ///     Gerenciador de Carga
        /// </summary>
        public readonly Manager Manager;

        /// <summary>
        ///     Cliente de Agendamento de Trabalho BIPBOP
        /// </summary>
        /// <param name="bipbopClient">Cliente da API</param>
        /// <param name="manager">Gerenciador de Carga</param>
        public Client(BipbopNet.Client bipbopClient = null, Manager manager = null)
        {
            Manager = manager ?? Manager.Default;
            BipbopClient = bipbopClient ?? new BipbopNet.Client();
        }

        /// <summary>
        ///     Recupera um Documento
        /// </summary>
        /// <param name="id">Identificador</param>
        /// <returns>Documento</returns>
        public async Task<BipbopDocument> Document(JobIdentifier id)
        {
            return await RequestPush($"SELECT FROM '{Manager}'.'DOCUMENT'", id);
        }

        /// <summary>
        ///     Recupera um Documento Deletado
        /// </summary>
        /// <param name="id">Identificador</param>
        /// <returns>Documento</returns>
        public async Task<BipbopDocument> DeletedDocument(JobIdentifier id)
        {
            return await RequestPush($"SELECT FROM '{Manager}'.'DELETEDDOCUMENT'", id);
        }

        /// <summary>
        ///     Deleta um Trabalho
        /// </summary>
        /// <param name="id">Identificador</param>
        public async Task Delete(JobIdentifier id)
        {
            await RequestPush($"DELETE FROM '{Manager}'.'JOB'", id);
        }

        /// <summary>
        ///     Retorna o Status de um Trabalho Deletado
        /// </summary>
        /// <param name="configuration">Identificador</param>
        /// <returns>Status do Trabalho</returns>
        public Task<Status> DeletedStatus(JobIdentifier configuration)
        {
            return AbstractStatus("DELETEDJOB", configuration);
        }

        /// <summary>
        ///     Retorna o Status de um Trabalho
        /// </summary>
        /// <param name="configuration">Identificador</param>
        /// <returns>Status do Trabalho</returns>
        public Task<Status> Status(JobIdentifier configuration)
        {
            return AbstractStatus("JOB", configuration);
        }

        private async Task<Status> AbstractStatus(string target, JobIdentifier configuration)
        {
            var doc = await RequestPush($"SELECT FROM '{Manager}'.'{target}'", configuration);
            var pushObject = doc.Document.SelectSingleNode("/BPQL/body/pushObject");
            return pushObject == null ? null : ParsePushObject(pushObject);
        }

        private async Task<BipbopDocument> RequestPush(string query, JobIdentifier configuration)
        {
            var requestParameter = new List<KeyValuePair<string, string>>();

            if (configuration.Id != null)
                requestParameter.Add(new KeyValuePair<string, string>("id", configuration.Id));
            if (configuration.Label != null)
                requestParameter.Add(new KeyValuePair<string, string>("label", configuration.Label));

            var doc = await BipbopClient.Request(query, requestParameter);
            return doc;
        }

        private int? ParseInt(string value)
        {
            if (value == null) return null;
            return int.Parse(value);
        }

        private DateTime? ParseTime(string value)
        {
            if (value == null) return null;
            return DateTime.Parse(value);
        }

        private Status ParsePushObject(XmlNode selectSingleNode)
        {
            return new Status
            {
                Job = new JobIdentifier
                {
                    Id = selectSingleNode.SelectSingleNode("./id")?.InnerText,
                    Label = selectSingleNode.SelectSingleNode("./label")?.InnerText
                },
                Machine = selectSingleNode.SelectSingleNode("./machine/node[1]")?.InnerText,
                Pid = ParseInt(selectSingleNode.SelectSingleNode("./machine/node[2]")?.InnerText),

                Version = ParseInt(selectSingleNode.SelectSingleNode("./version")?.InnerText),
                Tries = ParseInt(selectSingleNode.SelectSingleNode("./trys")?.InnerText) /* deprecated typo */
                        ?? ParseInt(selectSingleNode.SelectSingleNode("./tries")?.InnerText),
                Executions = ParseInt(selectSingleNode.SelectSingleNode("./executions")?.InnerText),
                SuccessExecutions = ParseInt(selectSingleNode.SelectSingleNode("./successExecutions")?.InnerText),
                HasException = selectSingleNode.SelectSingleNode("./hasException")?.InnerText == "true",
                Locked = selectSingleNode.SelectSingleNode("./locked")?.InnerText != "false",
                Processing = selectSingleNode.SelectSingleNode("./processing")?.InnerText != "false",
                Callback = selectSingleNode.SelectSingleNode("./juristekCallback/node")?.InnerText ??
                           selectSingleNode.SelectSingleNode("./callback/node")?.InnerText,
                Exception = null,
                Created = ParseTime(selectSingleNode.SelectSingleNode("./created")?.InnerText),
                NextJob = ParseTime(selectSingleNode.SelectSingleNode("./nextJob")?.InnerText),
                ProcessingAt = ParseTime(selectSingleNode.SelectSingleNode("./processingAt")?.InnerText),
                LastRun = ParseTime(selectSingleNode.SelectSingleNode("./lastRun")?.InnerText),
                LastSuccessRun = ParseTime(selectSingleNode.SelectSingleNode("./lastSuccessRun")?.InnerText),
                Deleted = ParseTime(selectSingleNode.SelectSingleNode("./deleted")?.InnerText),
                ExpectedNextJob = ParseTime(selectSingleNode.SelectSingleNode("./expectedNextJob")?.InnerText)
            };
        }

        /// <summary>
        ///     Cria um Trabalho
        /// </summary>
        /// <param name="userConfiguration">Trabalho</param>
        /// <returns>Trabalho</returns>
        public async Task<Configuration> Create(Configuration userConfiguration)
        {
            var configuration = (Configuration) userConfiguration.Clone();
            var sendParameters = KeyValuePairs(configuration);
            var doc = await BipbopClient.Request($"INSERT INTO '{Manager}'.'JOB'", sendParameters.ToList());
            configuration.Id = doc.Document.SelectSingleNode("/BPQL/body/id")?.InnerText;
            return configuration;
        }

        /// <summary>
        ///     Atualiza os Parâmetros de um Trabalho
        /// </summary>
        /// <param name="configuration">Parâmetros de Trabalho</param>
        /// <returns>Trabalho</returns>
        public async Task<Status> Update(Configuration configuration)
        {
            var sendParameters = KeyValuePairs(configuration);
            var doc = await BipbopClient.Request($"UPDATE '{Manager}'.'JOB'", sendParameters.ToList());
            return ParsePushObject(doc.Document.SelectSingleNode("/BPQL/body/pushObject"));
        }

        /// <summary>
        ///     Lista os Trabalhos
        /// </summary>
        /// <param name="listParameters">Filtro</param>
        /// <returns>Trabalhos</returns>
        public Task<StatusList> List(ListParameters listParameters)
        {
            return PushStatuses(listParameters, "JOB");
        }

        /// <summary>
        ///     Lista os Trabalhos Deletados
        /// </summary>
        /// <param name="listParameters">Filtro</param>
        /// <returns>Trabalhos</returns>
        public Task<StatusList> DeletedList(ListParameters listParameters)
        {
            return PushStatuses(listParameters, "DELETEDJOB");
        }

        private async Task<StatusList> PushStatuses(ListParameters listParameters, string target)
        {
            var filter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("limit", listParameters.Limit.ToString()),
                new KeyValuePair<string, string>("skip", listParameters.Skip.ToString())
            };
            if (listParameters.LastId != null)
                filter.Add(new KeyValuePair<string, string>("LastId", listParameters.LastId));

            if (listParameters.Version != null)
                filter.Add(new KeyValuePair<string, string>("Version", listParameters.Version.ToString()));

            if (listParameters.Tag != null)
                filter.Add(new KeyValuePair<string, string>("tag", listParameters.Tag));

            if (listParameters.Id != null)
                filter.Add(new KeyValuePair<string, string>("id", listParameters.Tag));

            if (listParameters.Label != null)
                filter.Add(new KeyValuePair<string, string>("Label", listParameters.Tag));

            var doc = await BipbopClient.Request($"SELECT FROM '{Manager}'.'{target}'", filter);
            var statusList = (from XmlNode i in doc.Document.SelectNodes("/BPQL/body/pushObject")
                    select ParsePushObject(i))
                .ToArray();

            var limitNode = doc.Document.SelectSingleNode("/BPQL/body/limit");
            var skipNode = doc.Document.SelectSingleNode("/BPQL/body/skip");
            var totalNode = doc.Document.SelectSingleNode("/BPQL/body/total");

            return new StatusList(
                limitNode != null ? int.Parse(limitNode.InnerText) : 0,
                skipNode != null ? int.Parse(skipNode.InnerText) : 0,
                totalNode != null ? int.Parse(totalNode.InnerText) : 0,
                statusList);
        }

        private static IEnumerable<KeyValuePair<string, string>> KeyValuePairs(Configuration configuration)
        {
            var requestParameter = new List<KeyValuePair<string, string>>();

            if (configuration.Callback != null)
                requestParameter.Add(new KeyValuePair<string, string>("pushCallback", configuration.Callback.ToString()));
            if (configuration.Query != null)
                requestParameter.Add(new KeyValuePair<string, string>("pushQuery", configuration.Query));
            if (configuration.Tags != null)
                requestParameter.Add(
                    new KeyValuePair<string, string>("pushTags", string.Join(",", configuration.Tags)));
            if (configuration.Label != null)
                requestParameter.Add(new KeyValuePair<string, string>("pushLabel", configuration.Label));
            if (configuration.At != null)
                requestParameter.Add(new KeyValuePair<string, string>("pushAt",
                    ((int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString()));
            if (configuration.Expire != null)
                requestParameter.Add(new KeyValuePair<string, string>("pushExpire",
                    ((int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString()));

            if (configuration.MaxVersion != null)
                requestParameter.Add(
                    new KeyValuePair<string, string>("pushMaxVersion", configuration.MaxVersion.ToString()));
            if (configuration.Interval != null)
                requestParameter.Add(
                    new KeyValuePair<string, string>("pushInterval", configuration.Interval.ToString()));
            if (configuration.Priority != null)
                requestParameter.Add(
                    new KeyValuePair<string, string>("pushPriority", configuration.Priority.ToString()));
            if (configuration.RetryIn != null)
                requestParameter.Add(new KeyValuePair<string, string>("pushRetryIn", configuration.RetryIn.ToString()));

            requestParameter.Add(new KeyValuePair<string, string>("pushWebSocketDeliver",
                configuration.WebSocketDeliver != null && configuration.WebSocketDeliver == false ? "false" : "true"));

            var sendParameters = configuration.Parameters != null
                ? requestParameter.Concat(configuration.Parameters)
                : requestParameter;
            return sendParameters;
        }
    }
}