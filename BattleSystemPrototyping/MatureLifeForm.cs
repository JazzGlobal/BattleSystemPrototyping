using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace BattleSystemPrototyping
{
    [Serializable]
    public class MatureLifeForm : LifeForm
    {
        List<Limb> limbs;
        public MatureLifeForm(string name, int level, int maxHealth, int physicalAttack, int magicalAttack,
                              int speed, int physicalDefense, int magicalDefense, List<Move> moveList, Attributes.AttributeTypes attributeType,
                              int healthGrowth, int physicalAttackGrowth, int magicalAttackGrowth, int speedGrowth, int physicalDefenseGrowth,
                              int magicalDefenseGrowth, List<Limb> limbs)
        {
            experience = 0;
            this.maxHealth = maxHealth;
            this.name = name;
            this.level = level;
            this.physicalAttack = physicalAttack;
            this.magicalAttack = magicalAttack;
            this.speed = speed;
            this.physicalDefense = physicalDefense;
            this.magicalDefense = magicalDefense;
            this.moveList = moveList;
            this.attributeType = attributeType;
            this.healthGrowth = healthGrowth;
            this.physicalAttackGrowth = physicalAttackGrowth;
            this.magicalAttackGrowth = magicalAttackGrowth;
            this.speedGrowth = speedGrowth;
            this.physicalDefenseGrowth = physicalDefenseGrowth;
            this.magicalDefenseGrowth = magicalDefenseGrowth;
            this.limbs = limbs;

            foreach (Limb limb in limbs)
            {
                this.maxHealth += limb.MaxHealth;
            }
        }
        public MatureLifeForm(string xmlPath, int id)
        {
            XElement doc = XElement.Load(xmlPath);
            foreach (var creature in doc.Elements())
            {
                if (creature.Attribute("id").Value == id.ToString())
                {
                    experience = 0;
                    maxHealth = Int32.Parse(creature.Element("health").Value);
                    name = creature.Element("name").Value;
                    level = 1;
                    physicalAttack = Int32.Parse(creature.Element("physicalattack").Value);
                    magicalAttack = Int32.Parse(creature.Element("magicalattack").Value);
                    speed = Int32.Parse(creature.Element("speed").Value);
                    physicalDefense = Int32.Parse(creature.Element("physicaldefense").Value);
                    magicalDefense = Int32.Parse(creature.Element("magicaldefense").Value);
                    moveList = new List<Move>();
                    attributeType = (Attributes.AttributeTypes)Int32.Parse(creature.Element("attributetype").Value);
                    healthGrowth = Int32.Parse(creature.Element("healthgrowth").Value);
                    physicalAttackGrowth = Int32.Parse(creature.Element("physicalattackgrowth").Value);
                    magicalAttackGrowth = Int32.Parse(creature.Element("magicalattackgrowth").Value);
                    speedGrowth = Int32.Parse(creature.Element("speedgrowth").Value);
                    physicalDefenseGrowth = Int32.Parse(creature.Element("physicaldefensegrowth").Value);
                    magicalDefenseGrowth = Int32.Parse(creature.Element("magicaldefensegrowth").Value);
                }
            }
            if (name == null)
            {
                throw new Exception();
            }
        }

        public int Level { get => level; }
        public long Experience { get => experience; }
        public string Name { get => name; }
        public int CurrentHealth
        {
            get
            {
                int health = 0;
                foreach (Limb limb in limbs)
                {
                    health += limb.CurrentHealth;
                }
                return health;
            }
        }
        public int MaxHealth { get => maxHealth; }
        public int PhysicalAttack { get => physicalAttack; }
        public int MagicalAttack { get => magicalAttack; }
        public int Speed { get => speed; }
        public int PhysicalDefense { get => physicalDefense; }
        public int MagicalDefense { get => magicalDefense; }
        public List<Move> MoveList { get => moveList; }
        public Attributes.AttributeTypes AttributeType { get => attributeType; }
        public int HealthGrowth { get => healthGrowth; }
        public int PhysicalAttackGrowth { get => physicalAttackGrowth; }
        public int MagicalAttackGrowth { get => magicalAttackGrowth; }
        public int SpeedGrowth { get => speedGrowth; }
        public int PhysicalDefenseGrowth { get => physicalDefenseGrowth; }
        public int MagicalDefenseGrowth { get => magicalDefenseGrowth; }
        public List<Limb> Limbs { get => limbs; }

        public static void PrintActionDetails(MatureLifeForm user, MatureLifeForm target, Limb limb, Move.MoveTypes moveType, int damage)
        {
            Console.WriteLine($"\n\n {user.Name} attacked {target.Name}'s {limb.Name} for {damage} {moveType} damage! \n\n");
        }

        public void PrintDetails()
        {
            Console.WriteLine($"Name:{name}" +
                            $"\nLevel:{level}" +
                            $"\nExperience:{experience}/{GetNeededExperience(Level + 1)}" +
                            $"\nPhysical Attack:{physicalAttack}" +
                            $"\nMagical Attack:{magicalAttack}" +
                            $"\nSpeed:{speed}" +
                            $"\nPhysical Defense:{physicalDefense}" +
                            $"\nMagical Defense:{magicalDefense}");
        }


        public void Attack(MatureLifeForm target, Limb limb)
        {
            var brokenLimbs = Limbs.FindAll(x => x.IsBroken == true && x.LimbType == LimbType.Damage); // Get all broken Damage limbs.
            double attackPenalty = (brokenLimbs.Count / 10); // 10% damage penalty per broken damage limb.
            int damageDealt = (int)(physicalAttack - (physicalAttack * attackPenalty));

            limb.CurrentHealth -= damageDealt;
            PrintActionDetails(this, target, limb, Move.MoveTypes.Physical, damageDealt);
        }
        public void UseMove(MatureLifeForm target, Limb limb, Move move)
        {
            int additionalDamage = 0;

            switch (move.MoveType)
            {
                case Move.MoveTypes.Physical:
                    additionalDamage += (int)(physicalAttack * .4);
                    break;
                case Move.MoveTypes.Magical:
                    additionalDamage += (int)(magicalAttack * .5);
                    break;
            }

            int totalDamage = move.BaseDamage + additionalDamage;

            limb.CurrentHealth -= totalDamage;
            PrintActionDetails(this, target, limb, move.MoveType, totalDamage);
        }

        private void LevelUp()
        {
            level += 1;
            physicalAttack = IncreaseStat(physicalAttack, physicalAttackGrowth);
            magicalAttack = IncreaseStat(magicalAttack, magicalAttackGrowth);
            speed = IncreaseStat(speed, speedGrowth);
            physicalDefense = IncreaseStat(physicalDefense, physicalDefenseGrowth);
            magicalDefense = IncreaseStat(magicalDefense, magicalDefenseGrowth);
            foreach (Limb limb in limbs)
            {
                limb.MaxHealth = IncreaseStat(limb.MaxHealth, healthGrowth);
            }
        }
        private int IncreaseStat(int stat, int growthRate)
        {
            stat += ((growthRate * 4) + 9) / 10;
            return stat;
        }
        public void GiveExperience(int experience)
        {
            this.experience += experience;
            while (this.experience >= GetNeededExperience(level + 1))
            {
                LevelUp();
            }
        }
        public void GiveExperience(MatureLifeForm defeatedCreature)
        {

            experience += (int)((Math.Pow(level, 2) * 100) / 4);
            while (experience >= GetNeededExperience(level + 1))
            {
                LevelUp();
            }

        }
        public long GetNeededExperience(int level)
        {
            long neededExperience = (long)((Math.Pow(level, 2) * 100) / 2);
            return neededExperience;
        }
        public void FullHeal()
        {
            foreach (Limb limb in limbs)
            {
                limb.CurrentHealth = limb.MaxHealth;
            }
        }
        public override bool IsAlive()
        {
            return CurrentHealth > 0;
        }

        private int level;
        private long experience;
        private string name;
        private int maxHealth;
        private int physicalAttack;
        private int magicalAttack;
        private int speed;
        private int physicalDefense;
        private int magicalDefense;
        private int healthGrowth;
        private int physicalAttackGrowth;
        private int magicalAttackGrowth;
        private int speedGrowth;
        private int physicalDefenseGrowth;
        private int magicalDefenseGrowth;
        private List<Move> moveList;
        private Attributes.AttributeTypes attributeType;
    }

    public enum LimbType
    {
        Mobility,
        Damage,
        Accuracy,
        Health
    }

    [Serializable]
    public class Limb
    {
        string name;
        int maxHealth;
        int currentHealth;
        LimbType limbType;
        public Limb(string name, int maxHealth, LimbType limbType)
        {
            this.name = name;
            this.maxHealth = maxHealth;
            this.limbType = limbType;
            this.currentHealth = maxHealth;
        }

        public string Name { get => name; }
        public int MaxHealth { get => maxHealth; set => maxHealth = value; }
        public int CurrentHealth { get => currentHealth; set => currentHealth = value; }
        public LimbType LimbType { get => limbType; }
        public bool IsBroken
        {
            get { return currentHealth <= 0; }
        }
    }
}
