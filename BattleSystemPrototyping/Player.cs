using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace BattleSystemPrototyping
{
    [Serializable]
    public class Player
    {
        public int Currency { get { return currency; } }
        public List<LifeForm> OwnedLifeForms { get { return ownedLifeForms; } }
        public string Name { get { return name; } set { name = value; } }

        public Player(int initialCurrency, string name = "Player")
        {
            currency = initialCurrency;
            ownedLifeForms = new List<LifeForm>();
            this.name = name;
        }
        public void GiveCurrency(int amount)
        {
            currency += amount;
        }

        public static void Serialize(Player player, string name)
        {
            Stream s = File.Open($"{name}.dat-{DateTime.Now.ToString("dd-MM-yyyy hh-mm-ss")}", FileMode.OpenOrCreate);
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(s, player);
            s.Close();
        }
        public static void Deserialize(string path, ref Player player)
        {
            Stream s = File.Open(path, FileMode.Open);
            BinaryFormatter b = new BinaryFormatter();
            player = (Player)b.Deserialize(s);
            s.Close();
        }

        private int currency;
        private List<LifeForm> ownedLifeForms;
        private string name;
    }
}
