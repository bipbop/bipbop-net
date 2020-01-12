using System;

namespace BipbopNet.Push
{
    /// <summary>
    ///     Gerenciador de PUSH
    /// </summary>
    [Serializable]
    public class Manager
    {
        public readonly string Value;

        private Manager(string value)
        {
            Value = value;
        }

        /// <summary>
        ///     Processos de Baixo Custo
        /// </summary>
        public static Manager JuristekLowTenancy => new Manager("PUSHJURISTEKLOWTENANCY");

        /// <summary>
        ///     Processos Jurídicos
        /// </summary>
        public static Manager Juristek => new Manager("PUSHJURISTEK");

        /// <summary>
        ///     Documentos Diversos
        /// </summary>
        public static Manager Default => new Manager("PUSH");


        public override string ToString()
        {
            return Value;
        }
    }
}