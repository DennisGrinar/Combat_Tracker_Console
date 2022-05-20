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
            Console.WriteLine("N for Next Turn");
            Console.WriteLine("A to use Action");
            Console.WriteLine("B to use B.Action");
            Console.WriteLine("M to use Movement");// figure out how to add distance traveled
            Console.WriteLine("D for Damage");
            Console.WriteLine("H for Healing");
            Console.WriteLine("CS to cast spell");
            Console.WriteLine("Q to Quit");
        }
    }
    
}
