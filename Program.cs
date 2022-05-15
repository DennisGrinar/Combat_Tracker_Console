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
            Console.WriteLine("Heal for Healing");
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