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
            combat.PrintInitativeOrder();


            Console.WriteLine("Combat is starting!");
            PrintControls();
            
            var input =  Console.ReadLine();


            while (input != "Q")
            {
                switch (input)
                {
                    case "NT":
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
                        input = Console.ReadLine();
                        combat.GetCurrentInitCreature().Move(int.Parse(input));
                        break;
                    case "DAM":
                        Console.WriteLine("Who is taking damage?");
                        Console.WriteLine(combat.GetCreatureSelection());
                        input = Console.ReadLine();
                        var cre = EncounterList[(int.Parse(input) - 1)];// -1 to account for list having 0 index
                        Console.WriteLine("How much damage?");
                        input = Console.ReadLine();
                        amount = int.Parse(input); 
                        Console.WriteLine("Was it a critial hit? Type 'Y' or 'N'");
                        input = Console.ReadLine();
                        bool crit = false;
                        if(input == "Y") crit = true;
                        cre.TakeDamage(amount,crit);
                        break;
                    case "HEAL":
                        Console.WriteLine("How much healing?");
                        input = Console.ReadLine();
                        amount = int.Parse(input);                       
                        Console.WriteLine("Who is getting healing?");
                        Console.WriteLine(combat.GetCreatureSelection());
                        input = Console.ReadLine();
                        EncounterList[(int.Parse(input) - 1)].Heal(amount);// -1 to account for list having 0 index
                        break;

                    default:
                        Console.WriteLine("Please try again");
                        break;
                }
                combat.PrintInitativeOrder();
                PrintControls();
                input = Console.ReadLine();
            }
          
        }

        public static void PrintControls()
        {
            Console.WriteLine("NT for Next Turn");
            Console.WriteLine("A to use Action");
            Console.WriteLine("B to use B.Action");
            Console.WriteLine("M to use Movement");// figure out how to add distance traveled
            Console.WriteLine("DAM for Damage");
            Console.WriteLine("HEAL for Healing");
            Console.WriteLine("CS to cast spell");
            Console.WriteLine("Q to Quit");
        }
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