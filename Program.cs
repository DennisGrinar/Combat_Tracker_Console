namespace Combat_Tracker_Console
{

    class Program
    {
        static void Main(string[] args)
        {
            Dice die = new Dice();

            List<Creature> party = new List<Creature>()
            {
             new Creature("Bob", 30, 30, 15, 30, true, "" ),
             new Creature("Steve", 20, 35, 16, 20, true, ""),
             new Creature("Amy", 17, 35, 16, 30, true, ""),
             new Creature("Dustin", 40, 45, 15, 30, true, "")
            };

            List<Creature> enemies = new List<Creature>()
            {
             new Creature("Hobgoblin", 18, 18, 18, 30, false, ""),
             new Creature("Goblin A", 7, 7, 15, 30, false, "Group A"),
             new Creature("Goblin B", 7, 7, 15, 30, false, "Group A"),
             new Creature("Goblin C", 7, 7, 15, 30, false, "Group A")
            };


            Battle combat = new Battle(party, enemies);

            

            combat.RollInitive();
            ref List<Creature> EncounterList = ref combat.GetCreatures();
            int amount = 0;
            int creatureID = 0;

            combat.PrintInitativeOrder();


            Console.WriteLine("Combat is starting!");
            PrintControls();

            var command = InputString();

            while (command != "Q")
            {
                switch (command)
                {
                    case "N":
                        combat.EndTurn();
                        break;
                    case "A":
                        combat.GetCurrentInitCreature().UseAction();
                        break;
                    case "B":
                        combat.GetCurrentInitCreature().UseBonusAction();
                        break;
                    case "M":
                        Console.WriteLine($"How far? {combat.GetCurrentInitCreature().GetRemainingSpeed()} ft. left");
                        combat.GetCurrentInitCreature().Move(InputInt());
                        break;
                    case "D":
                        Console.WriteLine("Who is taking damage?");
                        Console.WriteLine(combat.GetCreatureSelection());
                        creatureID = InputInt(1,EncounterList.Count()) - 1;

                        Console.WriteLine("How much damage?");
                        amount = InputInt();

                        Console.WriteLine("Was it a critial hit? Type 'Y' or 'N'");

                        EncounterList[creatureID].TakeDamage(amount,InputBool());
                        
                        break;
                    case "H":
                        Console.WriteLine("How much healing?");
                        amount = InputInt();

                        Console.WriteLine("Who is getting healing?");
                        Console.WriteLine(combat.GetCreatureSelection());;
                        EncounterList[(InputInt(1,EncounterList.Count) - 1)].Heal(amount);// -1 to account for list having 0 index
                        
                        break;
                    case "C":

                        Console.WriteLine("Who is casting the spell?");
                        Console.WriteLine(combat.GetCreatureSelection());
                        creatureID = (InputInt(1, EncounterList.Count) - 1);

                        Console.WriteLine("What is the name of the spell?");
                        var spellName = InputString();

                        Console.WriteLine("Is it a concentration spell? Y/N");
                        bool yesOrNo = InputBool();


                        Console.WriteLine("How many targets does the spell effect?");
                        amount = InputInt(1, EncounterList.Count());
                        int[] charIds = new int[amount]; 

                        for (int i  = 0; i < amount; i++)
                        {
                            Console.WriteLine($"{amount - i} target(s) left. Pick a target");
                            combat.GetCreatureSelection();
                            charIds[i] = InputInt(1, EncounterList.Count()) - 1;// Figure out how to stop user frome selecting the same id twice
                        }

                         List<Creature> targets =  new List<Creature>();

                        foreach(var key in charIds)
                        {
                            targets.Add(EncounterList[key]);
                        }

                        Console.WriteLine("How may rounds does the spell last?");
                        amount = InputInt();

                        EncounterList[creatureID].CastSpell(spellName, yesOrNo, charIds, amount,ref EncounterList);
                        break;    

                    default:
                        Console.WriteLine("Please try again");
                        break;
                }
                combat.PrintInitativeOrder();
                PrintControls();
                command = InputString();
            }
          
        }

        public static void PrintControls()
        {
            Console.WriteLine("N for Next Turn");
            Console.WriteLine("A to use Action");
            Console.WriteLine("B to use B.Action");
            Console.WriteLine("M to use Movement");// figure out how to add distance traveled
            Console.WriteLine("D for Damage");
            Console.WriteLine("H for Healing");
            Console.WriteLine("C to cast spell");
            Console.WriteLine("Q to Quit");
        }

        private static string InputString()
        {
            var input = "";
            do
            {
                input = Console.ReadLine();
                if (input == null) Console.WriteLine("Please try again");
                else input.ToString().ToUpper();
            }while (input == null || input == "");

            return input;
        }

        private static int InputInt()
        {
            int num;
            var input = Console.ReadLine();
            while (!int.TryParse(input, out num))
            {
                Console.WriteLine("Invalid input. Please try again.");
                input = Console.ReadLine();
            }
            return num;
            
        }

        private static int InputInt(int min, int max)
        {
            int num;
            var input = Console.ReadLine();
            while (!int.TryParse(input, out num)|| num > max || num < min)
            {
                Console.WriteLine("Invalid input. Please try again.");
                input = Console.ReadLine();
            }
            return num;
        }

        private static bool InputBool()
        {
            var input = "";
            bool result = false;
            do
            {
                input = Console.ReadLine();
                if (input == "Y") result = true;
                else if (input == "N") result = false;
                else Console.WriteLine("Invalid input. Please try again.");
            } while (input != "Y" || input != "N");

            return result;
        }
                /* try
                {
                    book.AddGrade(double.Parse(input));
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (FormatException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    // code that will run even if an Exception occurs.
                }
                */
}