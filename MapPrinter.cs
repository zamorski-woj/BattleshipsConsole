namespace Battleships
{
    public static class MapPrinter
    {
        public static void ShowBothMaps(Player p)
        {
            Console.WriteLine();
            Console.WriteLine("Your ships");
            ShowMap(p.Map, p.Opponent.LastMove);
            Console.WriteLine("Enemy ships");
            ShowMap(p.EnemyMap, p.LastMove);
        }

        public static void ShowMap(Map m, Tuple<int, int>? lastMove = null)
        {
            lastMove ??= new Tuple<int, int>(-1, -1);
            int mapSize = m.Grid.GetLength(0);
            PrintLetterRow(mapSize, lastMove.Item1);

            for (int y = 0; y < mapSize; y++)
            {
                bool highlight = false;
                if (lastMove.Item2 == y)
                {
                    highlight = true;
                }
                PrintOnMap(highlight, y.ToString(), ConsoleColor.Black, ConsoleColor.White);

                for (int x = 0; x < mapSize; x++)
                {
                    highlight = false;
                    if (lastMove.Item1 == x && lastMove.Item2 == y)
                    {
                        highlight = true;
                    }

                    switch (m.Grid[x, y])
                    {
                        case CellType.Unknown:
                            PrintOnMap(highlight, "?", ConsoleColor.White);
                            break;

                        case CellType.Water:
                            PrintOnMap(highlight);
                            break;

                        case CellType.Ship:
                            PrintOnMap(highlight, "#", ConsoleColor.Black);
                            break;

                        case CellType.Hit:
                            PrintOnMap(highlight, "*", ConsoleColor.DarkRed);
                            break;

                        case CellType.Sunken:
                            PrintOnMap(highlight, "X", ConsoleColor.DarkRed);
                            break;
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        private static void PrintLetterRow(int mapSize, int lastMove)
        {
            bool highlight = false;
            PrintOnMap(false, " ", ConsoleColor.Black, ConsoleColor.White);
            for (int x = 0; x < mapSize; x++)
            {
                if (lastMove == x)
                {
                    highlight = true;
                }
                PrintOnMap(highlight, Coordinates.StringFromInt(x), ConsoleColor.Black, ConsoleColor.White);
                highlight = false;
            }
            Console.WriteLine("");
            Console.ResetColor();
        }

        private static void PrintOnMap(bool highlight = false, string character = "~", ConsoleColor foregroundColor = ConsoleColor.DarkBlue, ConsoleColor backgroundColor = ConsoleColor.DarkCyan)
        {
            Console.BackgroundColor = backgroundColor;
            Console.ForegroundColor = foregroundColor;
            if (highlight)
            {
                Console.BackgroundColor = ConsoleColor.DarkYellow;
            }
            Console.Write(character);
            Console.ResetColor();
        }
    }
}