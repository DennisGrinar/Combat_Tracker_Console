namespace Combat_Tracker_Console
{

    class Program
    {
        static void Main(string[] args)
        {
            Dice die = new Dice();

            List<Creature> party = new List<Creature>()
            {           //Name  HP  MaxHP AC SPD, DS, Init Group
             new Creature("Bob", 30, 30, 15, 30, true, "" ),
             new Creature("Steve", 20, 35, 16, 25, true, ""),
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
            ref List<Spell> SpellList = ref combat.GetSpellTracker();
            int amount;
            int creatureID;

            combat.PrintInitativeOrder();

            Outputs.PrintControls();

            var command = Inputs.InputString("Combat is starting! \n Select a command");

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
                        amount = Inputs.InputInt($"How far? {combat.GetCurrentInitCreature().GetRemainingSpeed()} ft. left");
                        int maxDistance = combat.GetCurrentInitCreature().GetRemainingSpeed();
                        while (amount > maxDistance) 
                        {
                            amount = Inputs.InputInt("Distance is too far. Please try again");
                        }

                        combat.GetCurrentInitCreature().Move(amount);
                        break;
                    case "R":

                        creatureID = CreatureSelection("Who is using thier reaction?", combat);
                        EncounterList[creatureID].UseReaction();
                        break;

                    case "GU":
                        creatureID = CreatureSelection("Who is getting up from being Prone?", combat);
                        EncounterList[creatureID].GetUp(Inputs.InputBool("Will this cost them half their speed?"));

                        break;

                    case "D":
                        creatureID = CreatureSelection("Who is taking damage?", combat);

                        amount = Inputs.InputInt("How much damage?");

                        EncounterList[creatureID].TakeDamage(amount, ref SpellList);

                        break;
                    case "H":

                        amount = Inputs.InputInt("How much healing?");

                        creatureID = CreatureSelection("Who is getting healing?", combat);
                        EncounterList[creatureID].Heal(amount);

                        break;
                    case "CS":
                        creatureID = CreatureSelection("Who is casting the spell?", combat);

                        var spellName = Inputs.InputName("What is the name of the spell?");

                        bool conc = Inputs.InputBool("Is it a concentration spell?");
                        //Checking to see if the caster already has a concentration spell up and ends it.
                        if (conc)
                        {
                            if (EncounterList[creatureID].IsConcentrating())
                            {
                                combat.EndConcentrationSpell(creatureID);
                            }
                        }

                        var duration = Inputs.InputInt("How may rounds does the spell last?");

                        if (Inputs.InputBool("Does the spell target any creatures?"))
                        {
                            amount = Inputs.InputInt(1, EncounterList.Count, "How many targets does the spell effect?");
                            int[] charIds = new int[amount];

                            for (int i = 0; i < amount; i++)
                            {
                                charIds[i] = CreatureSelection($"{amount - i} target(s) left. Pick a target", combat);
                                // Figure out how to stop user frome selecting the same id twice
                            }

                            List<Creature> targets = new();

                            foreach (var key in charIds)
                            {
                                targets.Add(EncounterList[key]);
                            }

                            bool moveable = Inputs.InputBool("Is the spell moveable?");

                            SpellList.Add(new Spell (spellName, conc, duration, charIds, ref EncounterList, moveable, creatureID));
                        }
                        else SpellList.Add(new Spell(spellName, conc, duration, ref EncounterList, creatureID));
                        break;

                    default:
                        Outputs.Message("Please try again");
                        break;
                }
                combat.PrintInitativeOrder();
                Outputs.PrintControls();
                command = Inputs.InputString("Select a command");
            }
            

        }
        private static int CreatureSelection(string message,Battle combat)
        {
            Outputs.Message(combat.GetCreatureSelection());
            return (Inputs.InputInt(1, combat.GetCreatures().Count(), message) - 1); // -1 to account for List have a zero index
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
                }*/
    }           
}