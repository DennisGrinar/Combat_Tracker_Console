namespace Combat_Tracker_Console
{
 public class Spell
    {
        string Name {get;}
        bool concentration {get;}
        List<Creature> Targets = new List<Creature>();
        int duration {get;set;}
        bool CreatureTargets { get; }
        bool MoveableTarget { get; }

    

        public Spell(string name, bool conc, int dur, int[] targetIds, ref List<Creature> combatants, bool moveable) 
            {
                Name = name;
                concentration = conc;
                duration = dur;
                MoveableTarget = moveable;
                CreatureTargets = true;


                // Adds the spell to the targets' effective by list.
                foreach (var tar in targetIds)
                {
                       combatants[tar].AddSpellEffect(Name);
                       Targets.Add(combatants[tar]);
                }

            }

        public Spell(string name, bool conc, int dur)
        {
            Name = name;
            concentration = conc;
            duration = dur;
            CreatureTargets = false;
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
        }



    }
}