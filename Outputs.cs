using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Combat_Tracker_Console
{
    class Outputs
    {
        public static void Message(string mes)
        {
            Console.WriteLine(mes);
        }

        public static void Invalid()
        { Console.WriteLine("Invalid Input, Please try again"); }

        public static void PrintControls()
        {
            Outputs.Message("N for Next Turn");
            Outputs.Message("A to use Action");
            Outputs.Message("B to use B.Action");
            Outputs.Message("M to use Movement");// figure out how to add distance traveled
            Outputs.Message("R to use Reaction");
            Outputs.Message("D for Damage");
            Outputs.Message("H for Healing");
            Outputs.Message("CS to cast spell");
            Outputs.Message("Q to Quit");
        }   
    }
    
}
