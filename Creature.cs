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

      
        public static void TakeDamage(Creature name, int damage,bool crit)
        {
            
            if (name.Status.Contains("Dying"))//checking for instant death saving throw failures if creature is already dying.
            { 
                if (damage >= name.MaxHP) ChangeStatus(name,"Dead");
                name.DeathSaves.MakeDeathSavingThrow(false);
                if (crit) name.DeathSaves.MakeDeathSavingThrow(false);
            }
            

            else
            {
                //Creature getting hit with damage.
                name.HP =- damage;


                //TODO: check to see if concentrating check is needed.
                if (name.Concentrating)
                {

                } 

                // Drops below zero.
                if (name.HP <= 0) 
                {
                    
                    if (name.HP == 0) ChangeStatus(name,"Unconscious");

                    if (name.MaxHP + name.HP <= 0) ChangeStatus(name, "Dead");// check for instant death
                
                    name.HP = 0;
                
                    if (name.hasDeathSaves) ChangeStatus(name,"Dying");
                    else ChangeStatus(name, "Dead");

                }

                if (name.Concentrating == true)
                {
                    //user input roll
                    ConcentrationCheck(name, damage/*,roll*/);
                }
            }
        }

 
        
        public static void Heal(Creature name, int healing)
        {
            if (!name.Status.Contains("Dead")) // Healing doesn't work if creature is dead.
            {
                name.HP =+ healing;

                if(name.HP > name.MaxHP) name.HP = name.MaxHP; // HP can not go over Max HP.
                
                if (name.Status.Contains("Dying"))
                {
                    RemoveStatus(name,"Dying");
                    name.DeathSaves.ResetDeathSaves();
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
        }
        private static void ConcentrationCheck(Creature name, int damage/*, int roll*/)
        {
            int dc = 10;
            if (damage / 2 > 10) dc = damage / 2;        
        }

        // Status Changes
       private static void ChangeStatus(Creature name, string newStatus)
        {
            name.Status.Clear();
            name.Status.Add(newStatus);
        }
        private static void RemoveStatus(Creature name, string oldStatus)
        {
            name.Status.Remove(oldStatus);
            if (name.Status.Count() == 0) name.Status.Add("None");
        }

    }
    }
