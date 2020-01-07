namespace BipbopNet.Parser
{
    /// <summary>
    /// Próximo Apontamento para Alívio de Carga do Portal,
    /// só é utilizado caso o portal tenha horários de
    /// indisponibilidade.
    /// </summary>
    public class NextAppointment
    {
        /// <summary>
        /// Hora
        /// </summary>
        public readonly int? Hour;
        
        /// <summary>
        /// Minuto
        /// </summary>
        public readonly int? Minute;

        public NextAppointment(int? hour, int? minute)
        {
            Minute = minute;
            Hour = hour;
        }
    }
}