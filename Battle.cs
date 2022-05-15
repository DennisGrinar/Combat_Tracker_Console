namespace Combat_Tracker_Console
{
 public class Battle
    {
        Dice die = new Dice();
        List<Creature> Party = new List<Creature>();
        List<Creature> Encounter = new List<Creature>();
        List<Creature> InitativeOrder = new List<Creature>();

        int initativeTracker{get;set;}
        int TopOfInitative{get;set;}

        public Battle(List<Creature> party, List<Creature> encounter)
        {
            Party = party;
            Encounter = encounter;
            //Preparation Phase
            //Figure out what to do if two or more BattleInit are the same.
        }

         public void RollInitive()
        {
            InitativeOrder.AddRange(Party);

            // Insert way to input actual dice rolls
            // at least have it as an option
            InitativeOrder.AddRange(Encounter);
            

            List<Creature> groups = new List<Creature>();
            List<Creature> remove = new List<Creature>();

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


            TopOfInitative = (InitativeOrder[0]).BattleInit;
            initativeTracker = TopOfInitative;

        }

        public void PrintInitativeOrder()
        {
            foreach(var cre in InitativeOrder)
            {
                if (initativeTracker == cre.BattleInit) Console.WriteLine($"-> {cre.Info()}");
                else Console.WriteLine(cre.Info());
            }

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