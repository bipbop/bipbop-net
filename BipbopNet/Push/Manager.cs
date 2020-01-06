namespace BipbopNet.Push
{
    public class Manager
    {

        public readonly string Value;
        private Manager(string value) { Value = value; }

        
        public override string ToString()
        {
            return Value;
        }

        public static Manager JuristekLowTenancy => new Manager("PUSHJURISTEKLOWTENANCY");
        public static Manager Juristek => new Manager("PUSHJURISTEK");
        public static Manager Default => new Manager("PUSH");
    }
}