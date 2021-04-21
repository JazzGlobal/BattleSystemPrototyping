using System;

namespace BattleSystemPrototyping
{
    [Serializable]
    public class LifeForm
    {
        public float Hunger { get { return hunger; } }
        public float Hydration { get { return hydration; } }
        public float Boredom { get { return boredom; } }
        public float TimeAlive { get { return timeAlive; } }

        public LifeForm()
        {
            hunger = 100;
            hydration = 100;
            boredom = 100;
        }
        public void PrintStats()
        {
            Console.WriteLine($"Hunger: {hunger}\nHydration: {hydration}\nBoredom: {boredom}\nTime Alive: {timeAlive / 60} Minute(s)");
        }
        public void Decay(float decayValue, float timeElapsedSinceLastDecay = 0)
        {
            hunger -= decayValue;
            hydration -= decayValue;
            boredom -= decayValue;

            if (IsAlive() == false)
            {
                Console.Write("Your creature is dead!");
            }
            else
            {
                timeAlive += timeElapsedSinceLastDecay;
            }
        }
        public virtual bool IsAlive()
        {
            if (hunger <= -20 || hydration <= -20 || boredom <= -20)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public void AcceptNourishment(Nourishment nourishment)
        {
            switch (nourishment.NourishmentType)
            {
                case Nourishment.NourishmentTypes.Hunger:
                    hunger += nourishment.NourishmentValue;
                    if (hunger > 100)
                    {
                        hunger = 100;
                    }
                    break;
                case Nourishment.NourishmentTypes.Hydration:
                    hydration += nourishment.NourishmentValue;
                    if (hydration > 100)
                    {
                        hydration = 100;
                    }
                    break;
                case Nourishment.NourishmentTypes.Boredom:
                    boredom += nourishment.NourishmentValue;
                    if (boredom > 100)
                    {
                        boredom = 100;
                    }
                    break;
            }
        }

        private float hunger;
        private float hydration;
        private float boredom;
        private float timeAlive;
    }
}
