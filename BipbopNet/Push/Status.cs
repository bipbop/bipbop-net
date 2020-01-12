using System;
using Exception = BipbopNet.Parser.Exception;

namespace BipbopNet.Push
{
    /// <summary>
    ///     Retorna as condições do PUSH
    /// </summary>
    [Serializable]
    public class Status
    {
        /// <summary>
        ///     Callback HTTP ou S3 de onde o documento será enviado
        /// </summary>
        public string Callback;

        /// <summary>
        ///     Momento da Criação
        /// </summary>
        public DateTime? Created;

        /// <summary>
        ///     Momento da Deleção
        /// </summary>
        public DateTime? Deleted;

        /// <summary>
        ///     Exceção
        /// </summary>
        public Exception Exception;

        /// <summary>
        ///     Execuções Realizadas
        /// </summary>
        public int? Executions;

        /// <summary>
        ///     Hora da Próxima Consulta sem Realinhamento
        /// </summary>
        public DateTime? ExpectedNextJob;

        /// <summary>
        ///     Valida se há exceção
        /// </summary>
        public bool? HasException;

        /// <summary>
        ///     Identificador do JOB
        /// </summary>
        public JobIdentifier Job;

        /// <summary>
        ///     Última vez que rodou
        /// </summary>
        public DateTime? LastRun;

        /// <summary>
        ///     Última vez que rodou com sucesso
        /// </summary>
        public DateTime? LastSuccessRun;

        /// <summary>
        ///     Trabalho Bloqueado, não será executado ou está sendo executado
        /// </summary>
        public bool? Locked;

        /// <summary>
        ///     Local de Execução do JOB
        /// </summary>
        public string Machine;

        /// <summary>
        ///     Próximo Agendamento
        /// </summary>
        public DateTime? NextJob;

        /// <summary>
        ///     ID do processo do JOB
        /// </summary>
        public int? Pid;

        /// <summary>
        ///     Sendo processado no momento
        /// </summary>
        public bool? Processing;

        /// <summary>
        ///     Data de início do último processamento
        /// </summary>
        public DateTime? ProcessingAt;

        /// <summary>
        ///     Quantidade de Execuções com Sucesso
        /// </summary>
        public int? SuccessExecutions;

        /// <summary>
        ///     Quantidade de Tentativas com Erro
        /// </summary>
        public int? Tries;

        /// <summary>
        ///     Quantidade de Versões do Documento
        /// </summary>
        public int? Version;
    }
}