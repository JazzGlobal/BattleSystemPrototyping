namespace BattleSystemPrototyping
{
    public class Nourishment
    {
        public enum NourishmentTypes
        {
            Hunger = 0,
            Hydration = 1,
            Boredom = 2
        }
        public string Name { get { return name; } }
        public NourishmentTypes NourishmentType { get { return nourishmentType; } }
        public float NourishmentValue { get { return nourishmentValue; } }
        public int Cost { get { return cost; } }

        private string name;
        private NourishmentTypes nourishmentType;
        private float nourishmentValue;
        private int cost;
        public Nourishment(string name, NourishmentTypes nourishmentType, float nourishmentValue, int cost)
        {
            this.name = name;
            this.nourishmentType = nourishmentType;
            this.nourishmentValue = nourishmentValue;
            this.cost = cost;
        }
    }
}