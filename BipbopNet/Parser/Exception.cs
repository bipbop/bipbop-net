#nullable enable

namespace BipbopNet.Parser
{
    public class Exception: System.Exception
    {
        public enum Codes
        {
            EmptyCode = -1,
            InternalUserBlocked= 0, // Usuário Bloqueado
            TableNotFound= 1, // Tipo de consulta inexistente
            DatabaseNotFound= 1, // Tribunal não suportado
            NotFound= 2, // Consulta não encontrada
            InvalidArgument= 3, // Parâmetro de consulta inválido
            SyntaxError= 3, // Erro de sintaxe
            MissingArgument= 4, // Parâmetro de consulta obrigatório inexistente
            MultipleResultsFound= 5, // Consulta com múltiplos resultados
            BlockedIp= 6, // IP do robô bloqueado
            UnexpectedHttpCode= 7, // Erro inesperado durante consulta ao site
            Timeout= 7, // Ocorreu timeout durante a consulta
            LoadPageFailed= 7, // Erro de carregamento de página
            ProxyConfigurationError= 7, // Erro de configuração de proxy
            RemoteSiteUnderMaintenance= 8, // Site sob manutenção
            AuthenticationFailure= 9, // Erro ao logar no site
            InternalServerError= 11,
            InternalError= 11, // Erro interno
            QueryLimit= 12, // Limite de consulta atingido
            PasswordRequired= 20, // Requer parâmetros de senha para acessar
            JusticeSecret= 21, // Segredo de Justiça
            ExpectedDataNotFound= 24, // Erro durante a obtenção dos dados
            CaptchaBreakFailed= 25, // Não foi possível quebrar o captcha
            InternalPushLabel= 26, // Já existe um PUSH com essa LABEL
            UnderMaintenance= 27, // Nossas manutenções
            SiteMessage= 28, // Mensagem enviada pelo portal
            BlockedByConfig= 29, // Bloqueado por uma configuração
            LegalReview= 30, // Revisão de Advogado
            ResourceUnavailable= 31, // Recurso não disponível
            InternalEmailUnchecked= 32, // Email interno não verificado
            InternalNotReady= 33,
            Outdated= 1522,
            ArchivedProcess= 1522,
            WithoutProceedings= 1523,
            Unknown= 1524,
            EmailUnchecked= 1525,
            BlockedUser= 1526,
        }

        public Exception(string message): base(message)
        {
        }
    }
}