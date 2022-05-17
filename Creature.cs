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
            this.StartTurn();
            Concentrating = false;
            Status.Add("None");
            SpellEffects.Add("None");
            Concentrating = false;
            //

            InitGroup = group;
        }

        public String Info()
        {
            string info =
            $"{BattleInit} \t{Name} \t \tHP:{HP}/{MaxHP} \tAC:{AC} \tSPD:{RemainingSpeed}/{speed}\n" 
          + $"\t\t\tACTION {HasAction}\tB. ACTION {HasBonusAction} \t MOVEMENT {HasMovement} \t REACTION {HasReaction} \t Concenrating {Concentrating}\n"
          + $"\t\t\tStatus Effects: {this.GetListReadout(Status)} \t Spell Effects: {this.GetListReadout(SpellEffects)} \n"
          + $"\t\t\t Spells Cast:";

            if (this.SpellTracker.Count == 0) info += " None";
            else
            {
                foreach (var sp in SpellTracker)
                {
                   info += $"\t\t\t {sp.SpellName}\t\t Effecting: {sp.GetTargets()}\t\t Duration: {sp.GetRemainingDuration}\t Concentration: {sp.IsConcentrationSpell}\n";
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
            foreach(var status in Status)
            {
                if(count == 0) info += status;
                else {info += $", {status}";}
                count ++;
            }
            return info;
        }

        public void SetInitiative (int init)
        {
            this.BattleInit = init;
        }

      
        public void TakeDamage(int damage,bool crit)
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
                    if (Concentrating) this.EndConcentrationSpell();
                }
                // Drops below zero.
                if (HP < 0) 
                {
                    if (MaxHP + HP <= 0) ChangeStatus("Dead");// check for instant death

                    if (Concentrating) this.EndConcentrationSpell();
                    
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

        public void StartTurn()
        {
            this.RemainingSpeed = this.speed;
            this.HasMovement = true;
            this.HasAction = true;
            this.HasReaction = true;
            this.HasBonusAction = true;
            foreach (Spell sp in SpellTracker)
            {
                sp.EndRoundofSpell();
                if (sp.GetRemainingDuration() == 0) {sp.EndSpell();}
            }
        }

        public void EndTurn()
        {
            this.RemainingSpeed = 0;
            this.HasMovement = false;
            this.HasAction = false;
            this.HasBonusAction = false;
        }
        // Turn Actions
        public void Move(int distance) 
        {
          if (RemainingSpeed >= distance)  RemainingSpeed -= distance;
          if (RemainingSpeed == 0) HasMovement = false;
        }

        public void UseAction() {this.HasAction = false;}

        public void UseBonusAction() {this.HasBonusAction = false;}

        public void UseReaction() {this.HasReaction = false;}


        //Spell Upkeep
        public bool IsConcentrating() {return Concentrating;}
        public void StartConcentration() {this.Concentrating = true;}
        public void EndConcentration() {this.Concentrating = false;}
        //Effected by Spell
        public void AddSpellEffect(string spellName) 
        {
            if (this.SpellEffects.Contains("None")) this.SpellEffects.Remove("None");
            this.SpellEffects.Add(spellName);
        }

        public void RemoveSpellEffect(string spellName) 
        {
            this.SpellEffects.Remove(spellName);
            if (SpellEffects.Count == 0) SpellEffects.Add("None");
        }
        // Casted the spell
        public void AddToSpellTracker(Spell sp){SpellTracker.Add(sp);}

        public void RemoveFromSpellTracker(Spell sp){SpellTracker.Remove(sp);}

        public void EndConcentrationSpell()
        {
            foreach (Spell sp in SpellTracker)
            {
                if (sp.IsConcentrationSpell()) sp.EndSpell();
            }
            Concentrating = false;
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
