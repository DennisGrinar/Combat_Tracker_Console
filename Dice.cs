namespace Combat_Tracker_Console
{
    public class Dice
    { 
        Random die = new Random();  
    
        public int D4() {return die.Next(1,5);}
        public int D6() {return die.Next(1,7);}
        public int D8() {return die.Next(1,9);}
        public int D10() {return die.Next(1,11);}
        public int D12() {return die.Next(1,13);}
        public int D20() {return die.Next(1,21);}
        public int D100() {return die.Next(1,101);}
    }
}