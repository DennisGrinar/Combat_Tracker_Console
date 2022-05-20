using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Combat_Tracker_Console
{
    internal class Inputs
    {
        public static string InputString()
        {
            var input = "";
            do
            {
                input = Console.ReadLine();
                if (input == null) Outputs.Invalid();
                else input.ToString().ToUpper();
            } while (input == null || input == "");

            return input.ToUpper();
        }
        public static string InputName()
        {
            var input = "";
            do
            {
                input = Console.ReadLine();
            } while (input == null || input == "");

            return input;
        }

        public static int InputInt()
        {
            int num;
            var input = Console.ReadLine();
            while (!int.TryParse(input, out num))
            {
                Outputs.Invalid();
                input = Console.ReadLine();
            }
            return num;

        }

        public static int InputInt(int min, int max)
        {
            int num;
            var input = Console.ReadLine();
            while (!int.TryParse(input, out num) || num > max || num < min)
            {
                Outputs.Invalid();
                input = Console.ReadLine();
            }
            return num;
        }

        public static bool InputBool()
        {
            var input = "";
            bool result = false;
            do
            {
                input = Console.ReadLine();
                input = input.ToUpper();
                if (input == "Y") result = true;
                else if (input == "N") result = false;
                else Outputs.Invalid();
            } while (!(input == "Y" || input == "N"));

            return result;
        }
    }
}

