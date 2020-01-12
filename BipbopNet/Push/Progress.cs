using System.Threading.Tasks;

namespace BipbopNet.Push
{
    public class Progress
    {
        private readonly Client _client;
        private readonly string _tag;


        public Progress(Client client, string tag)
        {
            _client = client;
            _tag = tag;
        }

        public ListParameters ListParameters => new ListParameters {Tag = _tag};

        /// <summary>
        ///     Progresso dos Processos
        /// </summary>
        /// <returns>Objeto do Progresso dos Processos</returns>
        public Progress Processos()
        {
            return new Progress(_client, $"${ListParameters.Tag},OABPROCESSO");
        }

        /// <summary>
        ///     Progresso dos Tribunais
        /// </summary>
        /// <returns>Objeto do Progresso dos Tribunais</returns>
        public Progress Tribunais()
        {
            return new Progress(_client, $"${ListParameters.Tag},oab-consulta");
        }

        public async Task<double> calculate()
        {
            var list = await _client.List(ListParameters);
            if (list.Total == 0) return 1;

            var deletedList = await _client.DeletedList(ListParameters);
            if (deletedList.Total == 0) return 0;

            return (double) deletedList.Total / (deletedList.Total + list.Total);
        }
    }
}