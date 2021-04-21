using System;
using System.Collections.Generic;
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
                    } catch (Exception ex)
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
                            magicalDefenseGrowth: 3);
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
                magicalDefenseGrowth: 2);

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
                        playerCreature.Attack(enemy);
                        answered = true;
                        break;
                    case "2":
                        playerCreature.UseMove(enemy, playerCreature.MoveList[0]);
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
                        break;
                    case "help":
                        PrintAvailableCommands();
                        break;
                }
            }
        }
        private void EnemyAction()
        {
            if (rand.Next(0, 2) == 0)
            {
                enemy.Attack(playerCreature);
            }
            else
            {
                enemy.UseMove(playerCreature, enemy.MoveList[0]);
            }
        }
        private void PrintBattleDetails()
        {
            Console.WriteLine($"\n\n{playerCreature.Name}\t\t{enemy.Name}\nHealth: {playerCreature.CurrentHealth}\t\tHealth: {enemy.CurrentHealth}\nLevel: {playerCreature.Level}\t\tLevel: {enemy.Level}\n\n");
        }
        private void PrintAvailableCommands()
        {
            Console.WriteLine($"1: Attack\n2: {playerCreature.MoveList[0].Name}\nsave: Save your progress\ndetails: View current battle state\nstats: Print the player creature\'s stats.\nhelp: Print these commands again.");
        }
    }
}
