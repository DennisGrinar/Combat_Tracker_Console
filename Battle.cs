namespace Combat_Tracker_Console
{
 public class Battle
    {
        Dice die = new Dice();
        List<Creature> Party = new();
        List<Creature> Encounter = new();
        List<Creature> InitativeOrder = new();
        List<Spell> SpellTracker = new();

        int initativeTracker{get;set;}
        int TopOfInitative{get;set;}

        public Battle(List<Creature> party, List<Creature> encounter)
        {
            Party = party;
            Encounter = encounter;
            //Preparation Phase
            //Figure out what to do if two or more BattleInit are the same.
            initativeTracker = 0;
        }

         public void RollInitive()
        {
            InitativeOrder.AddRange(Party);

            // Insert way to input actual dice rolls
            // at least have it as an option
            InitativeOrder.AddRange(Encounter);
            

            List<Creature> groups = new ();
            List<Creature> remove = new ();

            foreach(var cre in InitativeOrder)
            {
                if ((cre.InitGroup).Equals("")) cre.SetInitiative(die.D20());
                else groups.Add(cre);    
            }

            while(groups.Count > 0)
            {
                int roll = die.D20();
                string groupName = groups[0].InitGroup;
                foreach(var cre in groups)
                {
                   if (groupName == cre.InitGroup)
                   {
                       cre.BattleInit = roll;
                       remove.Add(cre);
                   }
                }
                foreach(var cre in remove)
                {
                    groups.Remove(cre);
                }
            }

            InitativeOrder.Sort(( x, y) => y.BattleInit.CompareTo(x.BattleInit));


            TopOfInitative = InitativeOrder.Count - 1; //List is 0 index

            InitativeOrder[0].StartTurn();
        }

        public void PrintInitativeOrder()
        {
            foreach(var cre in InitativeOrder)
            {
                if (InitativeOrder[initativeTracker] == cre) Outputs.Message($"-> {cre.Info(ref SpellTracker)}");
                else Outputs.Message(cre.Info(ref SpellTracker));
            }

        } 

        public void EndTurn()
        {
            InitativeOrder[initativeTracker].EndTurn();
            initativeTracker ++;
            if (initativeTracker == InitativeOrder.Count()) initativeTracker = 0;
            InitativeOrder[initativeTracker].StartTurn();

            List<Spell> RemoveList = new();

            foreach(var sp in SpellTracker)
            {
                if (sp.GetCaster() == InitativeOrder[initativeTracker])
                {
                    sp.EndRoundofSpell();
                    if(sp.GetRemainingDuration() == 0)
                    {
                        if (sp.IsConcentrationSpell()) sp.GetCaster().EndConcentration();
                        RemoveList.Add(sp);
                    }
                }
            }
            foreach(var sp in RemoveList) { SpellTracker.Remove(sp); }
        }

        public void EndConcentrationSpell(int creatureID)
        {
            
            foreach (var sp in SpellTracker)
            {
                if (sp.IsConcentrationSpell() && sp.GetCaster() == InitativeOrder[creatureID]) EndSpell(sp);
            }
        }

        public void EndSpell(Spell removeSpell) { SpellTracker.Remove(removeSpell); }

        public ref List<Creature> GetCreatures() {return ref InitativeOrder;}
        public ref List<Spell> GetSpellTracker() {return ref SpellTracker;}    


        public Creature GetCurrentInitCreature() {return InitativeOrder[initativeTracker];}
        public Creature GetTargetCreature(int id) { return InitativeOrder[id];}

        public string GetCreatureSelection()
        {
            int i = 1;
            var info = "Enter the number on the left. \n";
            foreach(var cre in InitativeOrder)
            {
                info += $"{i} \t {cre.Name}\n";
                i++;
            }

            return info;
        }

        public void AddSpellToTracker(ref Spell sp)
        {
            SpellTracker.Add(sp);
        }


      /*  public async void Fight()
        {
            foreach(var c in InitativeOrder)
            {
                initativeTracker = c.BattleInit;
                c.StartTurn();
                //await c.EndTurn()
            }
        }
        */


    }
}