using System;

namespace BattleSystemPrototyping
{
    [Serializable]
    public class Move
    {
        public string Name { get => name; }
        public MoveTypes MoveType { get => moveType; }
        public Attributes.AttributeTypes AttributeType { get => attributeType; }
        public int BaseDamage { get => baseDamage; }

        public enum MoveTypes
        {
            Physical = 0,
            Magical = 1
        }

        public Move(string name, MoveTypes moveType, Attributes.AttributeTypes attributeType, int baseDamage)
        {
            this.name = name;
            this.moveType = moveType;
            this.attributeType = attributeType;
            this.baseDamage = baseDamage;
        }

        public virtual void OnUse(MatureLifeForm user, MatureLifeForm target)
        {
            throw new NotImplementedException();
        }

        private string name;
        private MoveTypes moveType;
        private Attributes.AttributeTypes attributeType;
        private int baseDamage;
    }
}
