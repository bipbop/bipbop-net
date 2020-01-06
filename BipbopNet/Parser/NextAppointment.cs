namespace BipbopNet.Parser
{
    public class NextAppointment
    {
        public readonly int? Hour;
        public readonly int? Minute;

        public NextAppointment(int? hour, int? minute)
        {
            Minute = minute;
            Hour = hour;
        }
    }
}