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


            Battle combat = new (party, enemies);



            combat.RollInitive();
            ref List<Creature> EncounterList = ref combat.GetCreatures();
            int amount;
            int creatureID;

            combat.PrintInitativeOrder();


            Outputs.Message("Combat is starting!");
            Outputs.PrintControls();

            var command = Inputs.InputString();

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
                        Outputs.Message($"How far? {combat.GetCurrentInitCreature().GetRemainingSpeed()} ft. left");
                        amount = Inputs.InputInt();
                        int maxDistance = combat.GetCurrentInitCreature().GetRemainingSpeed();
                        while (amount > maxDistance) 
                        {
                            Outputs.Message("Distance is too far. Please try again");
                            amount = Inputs.InputInt();
                        }

                        combat.GetCurrentInitCreature().Move(amount);
                        break;
                    case "R":
                        Outputs.Message("Who is using thier reaction?");
                        Outputs.Message(combat.GetCreatureSelection());
                        creatureID = Inputs.InputInt(1, EncounterList.Count()) - 1;
                        EncounterList[creatureID].UseReaction();
                        break;
                    case "D":
                        Outputs.Message("Who is taking damage?");
                        Outputs.Message(combat.GetCreatureSelection());
                        creatureID = Inputs.InputInt(1, EncounterList.Count()) - 1;

                        Outputs.Message("How much damage?");
                        amount = Inputs.InputInt();

                        Outputs.Message("Was it a critial hit? Type 'Y' or 'N'");

                        EncounterList[creatureID].TakeDamage(amount);

                        break;
                    case "H":
                        Outputs.Message("How much healing?");
                        amount = Inputs.InputInt();

                        Outputs.Message("Who is getting healing?");
                        Outputs.Message(combat.GetCreatureSelection()); ;
                        EncounterList[(Inputs.InputInt(1, EncounterList.Count) - 1)].Heal(amount);// -1 to account for list having 0 index

                        break;
                    case "CS":

                        Outputs.Message("Who is casting the spell?");
                        Outputs.Message(combat.GetCreatureSelection());
                        creatureID = (Inputs.InputInt(1, EncounterList.Count) - 1);

                        Outputs.Message("What is the name of the spell?");
                        var spellName = Inputs.InputName();

                        Outputs.Message("Is it a concentration spell? Y/N");
                        bool conc = Inputs.InputBool();

                        Outputs.Message("How may rounds does the spell last?");
                        amount = Inputs.InputInt();



                        Outputs.Message("Does the spell target any creatures? Y/N");
                        if (Inputs.InputBool())
                        {
                            Outputs.Message("How many targets does the spell effect?");
                            amount = Inputs.InputInt(1, EncounterList.Count());
                            int[] charIds = new int[amount];

                            for (int i = 0; i < amount; i++)
                            {
                                Outputs.Message($"{amount - i} target(s) left. Pick a target");
                                combat.GetCreatureSelection();
                                charIds[i] = Inputs.InputInt(1, EncounterList.Count()) - 1;// Figure out how to stop user frome selecting the same id twice
                            }

                            List<Creature> targets = new List<Creature>();

                            foreach (var key in charIds)
                            {
                                targets.Add(EncounterList[key]);
                            }

                            Outputs.Message("Is the spell moveable? Y/N");
                            bool moveable = Inputs.InputBool();

                            EncounterList[creatureID].CastSpell(spellName, conc, amount, charIds, ref EncounterList, moveable);
                        }
                        else EncounterList[creatureID].CastSpell(spellName, conc, amount);
                        break;

                    default:
                        Outputs.Message("Please try again");
                        break;
                }
                combat.PrintInitativeOrder();
                Outputs.PrintControls();
                command = Inputs.InputString();
            }

        }

        
                /* try
                {
                    book.AddGrade(double.Parse(input));
                }
                catch (ArgumentException ex)
                {
                    Outputs.Message(ex.Message);
                }
                catch (FormatException ex)
                {
                    Outputs.Message(ex.Message);
                }
                finally
                {
                    // code that will run even if an Exception occurs.
                }
                */
}