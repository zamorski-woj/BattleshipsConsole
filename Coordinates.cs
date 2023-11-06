using System.Text.RegularExpressions;

namespace Battleships
{
    public static class Coordinates
    {
        private static int IntFromLetter(string v)
        {
            return v switch
            {
                "a" or "A" => 0,
                "b" or "B" => 1,
                "c" or "C" => 2,
                "d" or "D" => 3,
                "e" or "E" => 4,
                "f" or "F" => 5,
                "g" or "G" => 6,
                "h" or "H" => 7,
                "i" or "I" => 8,
                _ => 9,
            };
        }

        public static bool NotValidOrder(string order)
        {
            if (Regex.IsMatch(order, @"^[a-jA-J][0-9]$") || Regex.IsMatch(order, @"^[0-9][a-jA-J]$"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static Tuple<int, int> CoordinatesFromString(string order)
        {
            int x = 0, y = 0;
            if (Regex.IsMatch(order, @"^[a-jA-J][0-9]$"))
            {
                x = IntFromLetter(order[0].ToString());
                y = int.Parse(order[1].ToString());
            }
            if (Regex.IsMatch(order, @"^[0-9][a-jA-J]$"))
            {
                y = int.Parse(order[0].ToString());
                x = IntFromLetter(order[1].ToString());
            }

            return new Tuple<int, int>(x, y);
        }

        public static string StringFromInt(int x)
        {
            return x switch
            {
                0 => "A",
                1 => "B",
                2 => "C",
                3 => "D",
                4 => "E",
                5 => "F",
                6 => "G",
                7 => "H",
                8 => "I",
                _ => "J",
            };
        }

        public static string StringFromCoordinates(Tuple<int, int> whereToShoot)
        {
            return StringFromInt(whereToShoot.Item1) + whereToShoot.Item2;
        }
    }
}