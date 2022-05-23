namespace Combat_Tracker_Console
{
 public class Spell
    {
        string Name {get;}
        bool concentration {get;}
        List<Creature>  Targets = new();
        int duration {get;set;}
        bool CreatureTargets { get; }
        bool MoveableTarget { get; }
        Creature Caster { get; }

    

        public Spell(string name, bool conc, int dur, int[] targetIds, ref List<Creature> combatants, bool moveable, int casterID) 
            {
                Name = name;
                concentration = conc;
                duration = dur;
                MoveableTarget = moveable;
                CreatureTargets = true;
                Caster = combatants[casterID];

            if (conc)
            {
                Caster.StartConcentration();
            }

                // Adds the spell to the targets' effective by list.
                foreach (var tar in targetIds)
                {
                    Targets.Add(combatants[tar]);
                }

            }

        public Spell(string name, bool conc, int dur, ref List<Creature> combatants, int casterID)
        {
            Name = name;
            concentration = conc;
            duration = dur;
            CreatureTargets = false;
            Caster = combatants[casterID];
        }



        // Getters

        public int GetRemainingDuration() {return duration;}

        public string GetName() {return Name;} 

        public bool IsConcentrationSpell() {return concentration;}

        public Creature GetCaster() { return Caster; }

        public List<Creature> GetTargets() {return Targets;}

        public string GetTargetsNames()
        {
            var info = "Noncreature Target(s)";

            if (CreatureTargets)
            {
                bool first = true;
                foreach (var target in Targets)
                {
                    if (first)
                    {
                        info = target.Name;
                        first = false;
                    }
                    else info += $", {target.Name}";
                }
            }

            
            return info;
        }

        public string GetTargetList()
        {
            var info = "Noncreature target";
            bool first = false;
            foreach (var cre in Targets)
            {
                if (first)
                {
                    info = cre.GetName();
                    first = false;
                }
                else { info += $", {cre.GetName()}"; }
            }

            return info;
        }

        public void RemoveSpellTargert(ref Creature cre) {Targets.Remove(cre);}

        
        // Functions
        public void EndRoundofSpell() {duration--;}
       




    }
}