using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace BattleSystemPrototyping
{
    public class PokemonBattleSystem
    {
        Player player;
        MatureLifeForm enemy;
        MatureLifeForm playerCreature;
        Random rand = new Random();
        public void StartGame()
        {

            if (player == null)
            {
                Console.WriteLine("ATTEMPTING TO LOAD PLAYER");
                // Attempt to load player. 
                try
                {
                    DirectoryInfo di = new DirectoryInfo(".");
                    FileSystemInfo[] files = di.GetFiles("*.dat")
                        .OrderBy(f => f.CreationTime)
                        .ToArray();
                    Player.Deserialize(files.Last().FullName, ref player);

                }
                catch (Exception e)
                {
                    player = new Player(50, "Chris");
                    MatureLifeForm temp;
                    Console.WriteLine("Which creature do you want to play? (Choose an ID. Example 1= Pyrastar)");
                    string input_id = Console.ReadLine();

                    try
                    {
                        temp = new MatureLifeForm("creatures.xml", Int32.Parse(input_id));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occured while trying to load creature ID: {0}. Defaulting to Pyrastar", input_id);
                        temp = new MatureLifeForm(
                            name: "Pyrastar",
                            level: 1,
                            maxHealth: 50,
                            physicalAttack: 3,
                            magicalAttack: 6,
                            speed: 5,
                            physicalDefense: 3,
                            magicalDefense: 3,
                            moveList: new List<Move>(),
                            attributeType: Attributes.AttributeTypes.Pyre,
                            healthGrowth: 1,
                            physicalAttackGrowth: 1,
                            magicalAttackGrowth: 3,
                            speedGrowth: 0,
                            physicalDefenseGrowth: 1,
                            magicalDefenseGrowth: 3,
                            new List<Limb>() {
                                new Limb("Left Leg", 10, LimbType.Mobility),
                                new Limb("Right Leg", 10, LimbType.Mobility),
                                new Limb("Torso", 50, LimbType.Health),
                                new Limb("Left Arm", 10, LimbType.Damage),
                                new Limb("Right Arm", 10, LimbType.Damage),
                                new Limb("Head", 20, LimbType.Accuracy)
                            });
                    }

                    player.OwnedLifeForms.Add(temp);
                    playerCreature = player.OwnedLifeForms[0] as MatureLifeForm;
                    playerCreature.MoveList.Add(new Move("Fire Stream", Move.MoveTypes.Magical, Attributes.AttributeTypes.Pyre, 10));
                }
            }

            playerCreature = player.OwnedLifeForms[0] as MatureLifeForm;
            playerCreature.FullHeal();

            enemy = new MatureLifeForm(
                name: "Electrostar",
                level: 1,
                maxHealth: 50,
                physicalAttack: 6,
                magicalAttack: 3,
                speed: 5,
                physicalDefense: 3,
                magicalDefense: 3,
                moveList: new List<Move>(),
                attributeType: Attributes.AttributeTypes.Shock,
                healthGrowth: 2,
                physicalAttackGrowth: 3,
                magicalAttackGrowth: 1,
                speedGrowth: 0,
                physicalDefenseGrowth: 2,
                magicalDefenseGrowth: 2,
                new List<Limb>() {
                    new Limb("Left Leg", 10, LimbType.Mobility),
                    new Limb("Right Leg", 10, LimbType.Mobility),
                    new Limb("Torso", 50, LimbType.Health),
                    new Limb("Left Arm", 10, LimbType.Damage),
                    new Limb("Right Arm", 10, LimbType.Damage),
                    new Limb("Head", 20, LimbType.Accuracy)
                    });

            enemy.MoveList.Add(new Move("Electro Pummel", Move.MoveTypes.Physical, Attributes.AttributeTypes.Shock, 5));
            PrintBattleDetails();
            PrintAvailableCommands();

            while (FightOngoing())
            {
                PlayerAction();
                if (!FightOngoing())
                {
                    playerCreature.GiveExperience(enemy);
                    Console.WriteLine($"{playerCreature.Name}\nLevel: {playerCreature.Level}\nExperience To Next Level: {playerCreature.Experience} / {playerCreature.GetNeededExperience(playerCreature.Level + 1)} ");
                    break;
                }
                EnemyAction();
                if (!FightOngoing())
                {
                    break;
                }
                PrintBattleDetails();
            }

            PrintBattleDetails();
            bool answered = false;
            while (answered == false)
            {
                Console.WriteLine("Battle Again? (Y / N)");
                string answer = Console.ReadLine().ToLower();
                switch (answer)
                {
                    case "y":
                        answered = true;
                        StartGame();
                        break;
                    default:
                        answered = true;
                        break;
                }
            }
        }

        private bool FightOngoing()
        {
            if (enemy.IsAlive() && playerCreature.IsAlive())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private Limb ChooseLimb(MatureLifeForm target)
        {
            Console.WriteLine($"Which of {target.Name}'s limbs should be targeted?");


            for (int i = 0; i < target.Limbs.Count; i++)
            {
                Console.WriteLine($"{i}: {target.Limbs[i].Name} (HP: {target.Limbs[i].CurrentHealth} / {target.Limbs[i].MaxHealth})");
            }
            string answer = Console.ReadLine();
            try
            {
                Limb limb = target.Limbs[Int32.Parse(answer)];
                if (limb.CurrentHealth <= 0)
                {
                    Console.WriteLine("That limb is broken! Pick another!");
                    ChooseLimb(target);
                }

                return limb;
            }
            catch (Exception ex)
            {
                if (ex is IndexOutOfRangeException)
                    Debug.WriteLine("Invalid index selected for limb. Returning first non broken limb.", ex);
                if (ex is FormatException)
                    Debug.WriteLine("Invalid input for limb entry. Returning first non broken limb.", ex);

                return ChooseNonBrokenLimb(target);
            }
        }

        private void PlayerAction()
        {
            bool answered = false;
            while (answered == false)
            {
                Console.WriteLine($"\n\n{playerCreature.Name}\'s turn!");
                string answer = Console.ReadLine();

                switch (answer)
                {
                    case "1":

                        playerCreature.Attack(enemy, ChooseLimb(enemy));
                        // Instead of just attacking the enemy, this is where we should ask the player which limb we want them to select for damage.
                        answered = true;
                        break;
                    case "2":
                        playerCreature.UseMove(enemy, ChooseLimb(enemy), playerCreature.MoveList[0]);
                        answered = true;
                        break;
                    case "save":
                        Player.Serialize(player, player.Name);
                        Console.WriteLine("Progress Saved!");
                        break;
                    case "details":
                        PrintBattleDetails();
                        Console.WriteLine($"{playerCreature.Name}\nLevel: {playerCreature.Level}\nExperience To Next Level: {playerCreature.Experience} / {playerCreature.GetNeededExperience(playerCreature.Level + 1)} ");
                        break;
                    case "stats":
                        playerCreature.PrintDetails();
                        enemy.PrintDetails();
                        break;
                    case "help":
                        PrintAvailableCommands();
                        break;
                }
            }
        }

        private Limb ChooseNonBrokenLimb(MatureLifeForm target)
        {
            foreach (Limb limb in target.Limbs)
            {
                if (limb.CurrentHealth > 0)
                {
                    return limb;
                }
            }
            return null;
        }

        private void EnemyAction()
        {
            if (rand.Next(0, 2) == 0)
            {
                enemy.Attack(playerCreature, ChooseNonBrokenLimb(playerCreature));
            }
            else
            {
                enemy.UseMove(playerCreature, ChooseNonBrokenLimb(playerCreature), enemy.MoveList[0]);
            }
        }
        private void PrintBattleDetails()
        {
            Console.WriteLine($"\n\n{playerCreature.Name}\t\t{enemy.Name}\nHealth: {playerCreature.CurrentHealth}\t\tHealth: {enemy.CurrentHealth}\nLevel: {playerCreature.Level}\t\tLevel: {enemy.Level}\n\n");
            Console.WriteLine($"{playerCreature.Name}'s Limb Details");
            foreach (Limb limb in playerCreature.Limbs)
            {
                Console.WriteLine($"{limb.Name} (HP: {limb.CurrentHealth} / {limb.MaxHealth})");
            }
            Console.WriteLine();
            Console.WriteLine($"{enemy.Name}'s Limb Details");
            foreach (Limb limb in enemy.Limbs)
            {
                Console.WriteLine($"{limb.Name} (HP: {limb.CurrentHealth} / {limb.MaxHealth})");
            }
        }
        private void PrintAvailableCommands()
        {
            Console.WriteLine($"1: Attack\n2: {playerCreature.MoveList[0].Name}\nsave: Save your progress\ndetails: View current battle state\nstats: Print the player creature\'s stats.\nhelp: Print these commands again.");
        }
    }
}
