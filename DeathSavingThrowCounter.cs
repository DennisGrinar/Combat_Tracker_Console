namespace Combat_Tracker_Console
{
    public class DeathSavingThrowCounter
    {
        int successes {get;set;}
        int failures {get;set;}
        
        
        public DeathSavingThrowCounter()
        {
            ResetDeathSaves();
        }

        
        
          public void ResetDeathSaves()
        {
            successes = 0;
            failures = 0;
        }

        public String MakeDeathSavingThrow(bool roll)
        {
            if (roll) successes++; else failures++;
            return CheckDeathSavingThrows();
        }

        public String MakeDeathSavingThrow(int roll)
        {
            if (roll > 10) successes++; else failures++;
            return CheckDeathSavingThrows();
        }

        private String CheckDeathSavingThrows()
        {
            if (successes >= 3) return "Unconscious";
            else if (failures >= 3) return "Dead";
            else return "Dying";
        }




        
    }
}