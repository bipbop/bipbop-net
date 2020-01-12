using System;
using System.Collections.Generic;
using System.Linq;

namespace BipbopNet.Push
{
    [Serializable]
    public class StatusList
    {
        /// <summary>
        ///     Pushs Salvos
        /// </summary>
        public readonly IEnumerable<Status> Items;

        /// <summary>
        ///     Pulando X elementos
        /// </summary>
        public readonly int Limit;

        /// <summary>
        ///     Pulando N elementos
        /// </summary>
        public readonly int Skip;

        /// <summary>
        ///     Total de Elementos
        /// </summary>
        public readonly int Total;

        public StatusList(int limit, int skip, int total, IEnumerable<Status> status)
        {
            Limit = limit;
            Skip = skip;
            Total = total;
            Items = status ?? throw new ArgumentNullException(nameof(status));
        }

        /// <summary>
        ///     Maior ID, última inserção da lista
        ///     Campo utilizado para paginação
        /// </summary>
        public string LastId
        {
            get { return Items.Max(a => a.Job.Id); }
        }

        /// <summary>
        ///     Recupera Próxima Página
        /// </summary>
        /// <param name="list">Lista de Resultados</param>
        /// <returns></returns>
        public ListParameters NextPage(ListParameters list)
        {
            var newList = (ListParameters) list.Clone();
            newList.LastId = LastId;
            return newList;
        }
    }
}