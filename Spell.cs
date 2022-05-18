namespace Combat_Tracker_Console
{
 public class Spell
    {
        string Name {get;}
        Creature Caster {get;}
        bool concentration {get;}
        List<Creature> Targets = new List<Creature>();
        int duration {get;set;}
    

        public Spell(string name, int casterId, bool conc, int[] targetIds, int dur, ref List<Creature> combatants) 
            {
                Name = name;
                Caster = combatants[casterId];
                concentration = conc;
                duration = dur;

                // Removes old Concentration Spell if caster has one up and is casting another.
                if (concentration) 
                {
                    if (Caster.IsConcentrating()) {Caster.EndConcentrationSpell();}
                    Caster.StartConcentration();
                }


                // Adds the spell to the targets' effective by list.
                foreach (var tar in targetIds)
                {
                       combatants[tar].AddSpellEffect(Name);
                       Targets.Add(combatants[tar]);
                }
                // Adds Spell to Caster's active list
                Caster.AddToSpellTracker(this);

            }

       
        // Getters
        public int GetRemainingDuration() {return duration;}

        public string SpellName() {return Name;} 

        public bool IsConcentrationSpell() {return concentration;}

        public string GetTargets()
        {
            string names = "";
            int i = 0;
            foreach(var target in Targets)
            {
                if (i == 0)
                    {names += target.Name;}
                else 
                    {names += ", " + target.Name;}
                i++;
            }

            return names;
        }

        
        // Functions
        public void EndRoundofSpell() {duration--;}
       
        public void EndSpell()
        {
            foreach (Creature c in Targets )
            {
                c.RemoveSpellEffect(this.Name);
            }
            if (concentration) Caster.EndConcentration();
            Caster.RemoveFromSpellTracker(this);
        }



    }
}