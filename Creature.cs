namespace Combat_Tracker_Console
{
        public class Creature
    {
        public string Name{get;}
        
        // HP info
        int MaxHP {get; set;}
        int TempHP {get; set;}
        int HP {get; set;}
        
        //AC info
        int AC {get; set;}
        int modAC {get;set;}
        
        //Movement Info
        int speed {get; set;}
        int modSpeed {get; set;}

        // Initial Order
        public int BattleInit {get; set;} 
        public string InitGroup {get; set;}
        
        // Turn Info
        bool HasReaction{get; set;}
        bool HasAction{get; set;}
        bool HasMovement{get; set;}
        int RemainingSpeed{get; set;}
        bool HasBonusAction{get; set;}
        
        //status info
        bool Concentrating{get; set;}
        List<string> Status = new List<string>();
        List<string> SpellEffects= new List<string>();
        bool hasDeathSaves {get;}
        DeathSavingThrowCounter DeathSaves  = new DeathSavingThrowCounter();

        List<Spell> SpellTracker = new List<Spell>();


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
            Concentrating = false;
            Status.Add("None");
            SpellEffects.Add("None");
            Concentrating = false;
            //

            InitGroup = group;
        }
        public Creature ()
        {
            Name ="";
            HP = 1;
            MaxHP = 1;
            AC = 0;
            speed = 0;
            hasDeathSaves = true;
            TempHP = 0;
            StartTurn();
            Concentrating = false;
            Status.Add("None");
            SpellEffects.Add("None");
            Concentrating = false;
            InitGroup = "";
        }

        public String Info()
        {
            string info =
            $"{BattleInit} \t{Name}  \tHP:{HP}/{MaxHP} \tAC:{AC} \tSPD:{RemainingSpeed}/{speed}\n" 
          + $"\tACTION {HasAction}\tBONUS ACTION {HasBonusAction} \t MOVEMENT {HasMovement} \t REACTION {HasReaction} \t Concenrating {Concentrating}\n"
          + $"\tStatus Effects: {this.GetListReadout(Status)} \t Spell Effects: {this.GetListReadout(SpellEffects)} \n"
          + $"\t Spells Cast:";

            if (this.SpellTracker.Count == 0) info += " None";
            else
            {
                foreach (var sp in SpellTracker)
                {
                   info += $"\n \t\t {sp.SpellName()}\t Effecting: {sp.GetTargets()}\t Duration: {sp.GetRemainingDuration()}\t Concentration: {sp.IsConcentrationSpell()}\n";
                }
            }

            info +=  $"\n";
            return info ;
        }

        public int GetRemainingSpeed(){return RemainingSpeed;}

        public string GetListReadout(List<string> fullList)
        {
            var info = "";
            int count = 0;
            foreach(var item in fullList)
            {
                if(count == 0) info += item;
                else {info += $", {item}";}
                count ++;
            }
            return info;
        }

        public void SetInitiative (int init)
        {
            BattleInit = init;
        }

      
        public void TakeDamage(int damage)
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
                Outputs.Message("Was the hit considered a crital hit?");
                bool crit = Inputs.InputBool();
                if (crit) DeathSaves.MakeDeathSavingThrow(false);
            }
            

            else
            {
                //Creature getting hit with damage.
                HP =- damage;

                if (HP > 0 && Concentrating) ConcentrationCheck(damage);
                
                if (HP == 0) 
                {
                    ChangeStatus("Unconscious");
                    if (Concentrating) EndConcentrationSpell();
                }
                // Drops below zero.
                if (HP < 0) 
                {//Work on this. If instant death, still runs through Death Saving Throw code.
                    if (MaxHP + HP <= 0) ChangeStatus("Dead");// check for instant death

                    if (Concentrating) EndConcentrationSpell();
                    
                    HP = 0;
                
                    if (hasDeathSaves) 
                    {
                        ChangeStatus("Dying");
                        AddStatus("Unconscious");
                    }
                    else ChangeStatus("Dead");

                }
            }
        }

        public void Heal(int healing)
        {
            if (!Status.Contains("Dead")) // Healing doesn't work if creature is dead.
            {
                HP =+ healing;

                if(HP > MaxHP) HP = MaxHP; // HP can not go over Max HP.
                
                if (Status.Contains("Dying"))
                {
                    RemoveStatus("Dying");
                    RemoveStatus("Unconscious");
                    DeathSaves.ResetDeathSaves();
                }
            }
        }

        public void CastSpell(string spellname, bool conc, int dur, int[] tar, ref List<Creature> combatants, bool moveable)
        {
            Spell sp = new(spellname, conc, dur, tar, ref combatants, moveable);

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


        public void StartTurn()
        {
            RemainingSpeed = speed;
            HasMovement = true;
            HasAction = true;
            HasReaction = true;
            HasBonusAction = true;
            foreach (Spell sp in SpellTracker)
            {
                sp.EndRoundofSpell();
                if (sp.GetRemainingDuration() == 0) 
                {
                    EndSpell(sp);
                }
            }
        }

        public void EndSpell(Spell sp)
        {
            if (sp.IsConcentrationSpell()) EndConcentration();
            sp.EndSpell();
            SpellTracker.Remove(sp);
        }

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
          if (RemainingSpeed >= distance)  RemainingSpeed -= distance;
          if (RemainingSpeed == 0) HasMovement = false;
        }

        public void UseAction() {HasAction = false;}

        public void UseBonusAction() {HasBonusAction = false;}

        public void UseReaction() {HasReaction = false;}


        //Spell Upkeep
        public bool IsConcentrating() {return Concentrating;}
        public void StartConcentration() {Concentrating = true;}
        public void EndConcentration() {Concentrating = false;}
        //Effected by Spell
        public void AddSpellEffect(string spellName) 
        {
            if (SpellEffects.Contains("None")) SpellEffects.Remove("None");
            this.SpellEffects.Add(spellName);
        }

        public void RemoveSpellEffect(string spellName) 
        {
            this.SpellEffects.Remove(spellName);
            if (SpellEffects.Count == 0) SpellEffects.Add("None");
        }

        public void EndConcentrationSpell()
        {
            bool needToEndSpell = false;
            int id = 0;
            int i = 0;

            //checking to see if there is a concentration spell up
            foreach (Spell sp in SpellTracker)
            {
                if (sp.IsConcentrationSpell())
                {
                    needToEndSpell = true;
                    id = i;
                }
                i++;
            }

            if (needToEndSpell) EndSpell(SpellTracker[id]);
        }
        private void ConcentrationCheck(int damage)
        {
            int dc = 10;
            if (damage / 2 > 10) dc = damage / 2;


            Console.WriteLine($"DC = {dc} What was the roll?");
            var input = Console.ReadLine();
            if (dc > int.Parse(input)) EndConcentrationSpell();
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
