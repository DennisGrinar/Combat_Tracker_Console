namespace Combat_Tracker_Console
{
 public class Spell
    {
        string Name {get;}
        Creature Caster {get;}
        bool concentration {get;}
        List<Creature> Targets = new List<Creature>();
        int duration {get;set;}
    

        public Spell(string name, Creature caster, bool conc, List<Creature> targets, int dur) 
            {
                Name = name;
                Caster = caster;
                concentration = conc;
                Targets = targets;
                duration = dur;

                // Removes old Concentration Spell if caster has one up and is casting another.
                if (concentration) 
                {
                    if (caster.IsConcentrating()) {caster.EndConcentrationSpell();}
                    caster.StartConcentration();
                }
                // Adds Spell to Caster's active list
                Caster.AddToSpellTracker(this);

                // Adds the spell to the targets' effective by list.
                foreach (var target in Targets)
                {
                    target.AddSpellEffect(this.Name);
                }

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