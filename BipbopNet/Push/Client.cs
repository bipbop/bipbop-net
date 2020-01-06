using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using BipbopNet.Parser;

namespace BipbopNet.Push
{
    public class Client
    {
        public readonly BipbopNet.Client BipbopClient;
        public readonly Manager? Manager;

        public Client(BipbopNet.Client bipbopClient, Manager? manager = null)
        {
            Manager = manager ?? Manager.Default;
            BipbopClient = bipbopClient;
        }

        public async Task<BipbopDocument> Document(PushIdentifier id)
        {
            return await RequestPush($"SELECT FROM '{Manager}'.'DOCUMENT'", id);
        }

        public async Task Delete(PushIdentifier id)
        {
            await RequestPush($"DELETE FROM '{Manager}'.'JOB'", id);
        }

        public async Task<PushStatus?> Status(PushIdentifier configuration)
        {
            var doc = await RequestPush($"SELECT FROM '{Manager}'.'JOB'", configuration);
            var pushObject = doc.Document.SelectSingleNode("/BPQL/body/pushObject");
            return pushObject == null ? null : ParsePushObject(pushObject);
        }

        private async Task<BipbopDocument> RequestPush(string query, PushIdentifier configuration)
        {
            var requestParameter = new List<KeyValuePair<string, string>>();

            if (configuration.Id != null)
                requestParameter.Add(new KeyValuePair<string, string>("id", configuration.Id));
            if (configuration.Label != null)
                requestParameter.Add(new KeyValuePair<string, string>("label", configuration.Label));

            var doc = await BipbopClient.Request(query, requestParameter);
            return doc;
        }

        private int? ParseInt(string? value)
        {
            if (value == null) return null;
            return int.Parse(value);
        }

        private DateTime? ParseTime(string? value)
        {
            if (value == null) return null;
            return DateTime.Parse(value);
        }

        private PushStatus ParsePushObject(XmlNode selectSingleNode)
        {
            return new PushStatus
            {
                Push = new PushIdentifier
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

        public async Task<PushConfiguration> Create(PushConfiguration userConfiguration)
        {
            var configuration = (PushConfiguration) userConfiguration.Clone();
            var sendParameters = KeyValuePairs(configuration);
            var doc = await BipbopClient.Request($"INSERT INTO '{Manager}'.'JOB'", sendParameters.ToList());
            configuration.Id = doc.Document.SelectSingleNode("/BPQL/body/id")?.InnerText;
            return configuration;
        }

        public async Task<PushStatus> Update(PushConfiguration configuration)
        {
            var sendParameters = KeyValuePairs(configuration);
            var doc = await BipbopClient.Request($"UPDATE '{Manager}'.'JOB'", sendParameters.ToList());
            return ParsePushObject(doc.Document.SelectSingleNode("/BPQL/body/pushObject"));
        }

        public async Task<PushStatus[]> List(ListParameters listParameters)
        {
            var filter = new List<KeyValuePair<string, string>>
            {
                KeyValuePair.Create("limit", listParameters.Limit.ToString()),
                KeyValuePair.Create("skip", listParameters.Skip.ToString())
            };
            if (listParameters.LastId != null) filter.Add(KeyValuePair.Create("LastId", listParameters.LastId));
            if (listParameters.Version != null)
                filter.Add(KeyValuePair.Create("Version", listParameters.Version.ToString()));
            if (listParameters.FilterTag != null) filter.Add(KeyValuePair.Create("tag", listParameters.FilterTag));

            var doc = await BipbopClient.Request($"SELECT FROM '{Manager}'.'JOB'", filter);
            return (from XmlNode i in doc.Document.SelectNodes("/BPQL/body/pushObject") select ParsePushObject(i))
                .ToArray();
        }

        private static IEnumerable<KeyValuePair<string, string>> KeyValuePairs(PushConfiguration configuration)
        {
            var requestParameter = new List<KeyValuePair<string, string>>();

            if (configuration.Callback != null)
                requestParameter.Add(new KeyValuePair<string, string>("pushCallback", configuration.Callback));
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
                configuration.WebSocketDeliver != null || configuration.WebSocketDeliver == false ? "false" : "true"));

            var sendParameters = configuration.Parameters != null
                ? requestParameter.Concat(configuration.Parameters)
                : requestParameter;
            return sendParameters;
        }
    }
}