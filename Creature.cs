namespace Combat_Tracker_Console
{
    public class Creature
    {
        public string Name { get; }

        // HP info
        int MaxHP { get; set; }
        int TempHP { get; set; }
        int HP { get; set; }

        //AC info
        int AC { get; set; }
        int modAC { get; set; }

        //Movement Info
        int speed { get; set; }
        int modSpeed { get; set; }

        // Initial Order
        public int BattleInit { get; set; }
        public string InitGroup { get; set; }

        // Turn Info
        bool HasReaction { get; set; }
        bool HasAction { get; set; }
        bool HasMovement { get; set; }
        int RemainingSpeed { get; set; }
        bool HasBonusAction { get; set; }

        //status info
        bool Concentrating { get; set; }
        List<string> Status = new();

        bool hasDeathSaves { get; }
        DeathSavingThrowCounter DeathSaves = new();




        public Creature(string name, int currentHP, int setHP, int ac, int spd, bool dSaves, string group)
        {

            //Find way to condense down
            // Adding user data.
            Name = name;
            HP = currentHP;
            MaxHP = setHP;
            AC = ac;
            speed = spd;
            hasDeathSaves = dSaves;
            // Setting start values.
            TempHP = 0;
            EndTurn();
            HasReaction = true;
            Concentrating = false;
            Status.Add("None");
            Concentrating = false;
            //

            InitGroup = group;
        }
        public Creature()
        {
            Name = "";
            HP = 1;
            MaxHP = 1;
            AC = 0;
            speed = 0;
            hasDeathSaves = true;
            TempHP = 0;
            StartTurn();
            Concentrating = false;
            Status.Add("None");
            Concentrating = false;
            InitGroup = "";
        }

        public int GetRemainingSpeed() { return RemainingSpeed; }

        public string GetName() { return Name; }

        public String Info(ref List<Spell> spellTracker)
        {
            string info =
            $"\t{BattleInit} \t{Name}  \tHP:{HP}/{MaxHP} \tAC:{AC} \tSPD:{RemainingSpeed}/{speed}\n"
          + $"\tACTION {HasAction}\tBONUS ACTION {HasBonusAction} \t MOVEMENT {HasMovement} \t REACTION {HasReaction} \t\t Concenrating {Concentrating}\n"
          + $"\tStatus Effects: {GetListReadout(Status)} \t"
          + $" Spell Effects: {GetSpellEffects(spellTracker)} \n"
          + $"\t Spells Cast:"
          + $"\t {GetSpellsCasted(spellTracker)}";




            info += $"\n";
            return info;
        }



        private string GetListReadout(List<string> fullList)
        {
            var info = "";
            int count = 0;
            foreach (var item in fullList)
            {
                if (count == 0) info += item;
                else { info += $", {item}"; }
                count++;
            }
            return info;
        }

        private string GetSpellEffects(List<Spell> spellTracker)
        {
            var info = "None";
            bool first = true;
            foreach (var sp in spellTracker)
            {
                if (sp.GetTargets().Contains(this))
                {
                    if (first) info = sp.GetName();
                    else { info += $", {sp.GetName()}"; }
                    first = false;
                }
            }
            return info;
        }

        private string GetSpellsCasted(List<Spell> spellTracker)
        {
            var info = "None";


            foreach(var sp in spellTracker)
            {
                if (sp.GetCaster() == this)
                {
                    info = $"\n \t\t {sp.GetName()}\t Effecting: {sp.GetTargetsNames()}\t Duration: {sp.GetRemainingDuration()}\t Concentration: {sp.IsConcentrationSpell()}\n";
                }
            }

            return info;
        }

        public void SetInitiative(int init)
        {
            BattleInit = init;
        }


        public void TakeDamage(int damage, ref List<Spell> spellTracker)
        {
            //Applying the rules for if a character is already dying.
            if (Status.Contains("Dying"))//checking for instant death saving throw failures if creature is already dying.
            {
                if (damage >= MaxHP)
                {
                    ChangeStatus("Dead");
                    DeathSaves.ResetDeathSaves();
                }
                DeathSaves.MakeDeathSavingThrow(false);
                bool crit = Inputs.InputBool("Was the hit considered a crital hit?");
                if (crit) DeathSaves.MakeDeathSavingThrow(false);
            }


            else
            {
                //Creature getting hit with damage.
                HP -= damage;

                if (HP > 0 && Concentrating) ConcentrationCheck(damage, ref spellTracker);

                if (HP <= 0)
                {
                    if (MaxHP + HP <= 0) ChangeStatus("Dead");// check for instant death
                    else if (HP == 0 && hasDeathSaves) ChangeStatus("Unconscious");

                    else if (hasDeathSaves)
                    {
                        ChangeStatus("Dying");
                        AddStatus("Unconscious");
                    }
                    else ChangeStatus("Dead");

                    if (Concentrating) EndConcentrationSpell(ref spellTracker);
                    HasAction = false;
                    HasBonusAction = false;
                    Concentrating = false;
                    HasMovement = false;
                    HasReaction = false;
                    RemainingSpeed = 0; 
      
                    HP = 0;
                }
            }
        }

        public void Heal(int healing)
        {
            if (!Status.Contains("Dead")) // Healing doesn't work if creature is dead.
            {
                HP = +healing;

                if (HP > MaxHP) HP = MaxHP; // HP can not go over Max HP.

                if (Status.Contains("Dying") || Status.Contains("Unconscious"))
                {
                    ChangeStatus("Prone");
                    DeathSaves.ResetDeathSaves();
                }
            }
        }

        public void GetUp (bool useSpeed)
        {
            if (Status.Contains("Prone"))
            {
                RemoveStatus("Prone");
                if (useSpeed)
                {
                    RemainingSpeed = RemainingSpeed / 2;
                    RemainingSpeed = RemainingSpeed - (RemainingSpeed % 5);
                }
            }
            else Outputs.Message("Combatant is not Prone");
        }

/*       public void CastSpell(string spellname, bool conc, int dur, int[] tar, ref List<Creature> combatants, bool moveable, int casterID)
        {
            Spell sp = new(spellname, conc, dur, tar, ref combatants, moveable, casterID);

            if (conc)
            {
                EndConcentrationSpell();
                StartConcentration();
            }

            SpellTracker.Add(sp);
        }
        public void CastSpell(string spellname, bool conc, int dur)
        {
            Spell sp = new(spellname, conc, dur);

            if (conc)
            {
                EndConcentrationSpell();
                StartConcentration();
            }

            SpellTracker.Add(sp);
        }

*/
        public void StartTurn()
        {
            RemainingSpeed = speed;
            HasMovement = true;
            HasAction = true;
            HasReaction = true;
            HasBonusAction = true;

        }

 /*       public void EndSpell(Spell sp)
        {
            if (sp.IsConcentrationSpell()) EndConcentration();
            sp.EndSpell();
            SpellTracker.Remove(sp);
        }
 */
        public void EndTurn()
        {
            HasMovement = false;
            RemainingSpeed = 0;
            HasAction = false;
            HasBonusAction = false;
        }
        // Turn Actions
        public void Move(int distance)
        {
            if (RemainingSpeed >= distance) RemainingSpeed -= distance;
            if (RemainingSpeed == 0) HasMovement = false;
        }

        public void UseAction() { HasAction = false; }

        public void UseBonusAction() { HasBonusAction = false; }

        public void UseReaction() { HasReaction = false; }


        //Spell Upkeep
        public bool IsConcentrating() { return Concentrating; }
        public void StartConcentration() { Concentrating = true; }
        public void EndConcentration() { Concentrating = false; }
        //Effected by Spell



        public void EndConcentrationSpell(ref List<Spell> spellTracker)
        {
            bool needToEndSpell = false;
            int id = 0;
            int i = 0;

            //checking to see if there is a concentration spell up
            foreach (Spell sp in spellTracker)
            {
                if (sp.GetCaster() == this && sp.IsConcentrationSpell())
                {
                    needToEndSpell = true;
                    id = i;
                }
                i++;
            }

            if (needToEndSpell) spellTracker.Remove(spellTracker[id]);
        }
        private void ConcentrationCheck(int damage, ref List<Spell> spellTracker)
        {
            int dc = 10;
            if (damage / 2 > 10) dc = damage / 2;


            Outputs.Message($"DC = {dc} What was the roll?");
            var input = Inputs.InputInt();
            if (dc > input) EndConcentrationSpell(ref spellTracker);
        }

        // Status Changes
        private void ChangeStatus(string newStatus)
        {
            Status.Clear();
            Status.Add(newStatus);
        }
        private void RemoveStatus(string oldStatus)
        {
            Status.Remove(oldStatus);
            if (Status.Count() == 0) Status.Add("None");
        }
        private void AddStatus(string newStatus)
        {
            Status.Add(newStatus);
        }



    }
}
