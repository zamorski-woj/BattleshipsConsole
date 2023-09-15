using System.Text.RegularExpressions;
using static Battleships.NPC;
using static Battleships.BattleshipsGame;

namespace Battleships
{
    public static class BattleshipsConsole
    {
        public static void Menu()
        {
            int numberOfPlayers = 2;
            string? decision = null;
            while (decision == null)
            {
                Console.WriteLine("How many human players? 0, 1 or 2?");
                decision = Console.ReadKey().KeyChar.ToString();
                try
                {
                    numberOfPlayers = int.Parse(decision);
                }
                catch (FormatException f)
                {
                    continue;//with default 2
                }
            }

            Console.Clear();
            Console.WriteLine("What to call the first player?");
            string name1 = Console.ReadLine() ?? "Ewaryst";
            Console.WriteLine("What to call the second player?");
            string name2 = Console.ReadLine() ?? "Antyfilidor";
            Console.Clear();

            Player winner = RunGame(numberOfPlayers, true, name1, name2);
            Console.WriteLine(winner.Name + " won!");
            Console.ReadLine();
            Console.Clear();
        }

        public static Player RunGame(int numberOfPlayers, bool showNPCsTurn = true, string name1 = "Ewaryst", string name2 = "Antyfilidor")
        {
            Tuple<Player, Player> bothPlayers = CreateTwoPlayers(name1, name2, 10);
            Player p1 = bothPlayers.Item1;
            Player p2 = bothPlayers.Item2;
            p1.GenerateFleet();
            p2.GenerateFleet();
            while (BothFleetsAlive(p1, p2))
            {
                switch (numberOfPlayers)
                {
                    case 0:
                        PlayAutomaticTurn(p1, p2, showNPCsTurn);
                        if (!BothFleetsAlive(p1, p2))
                        {
                            break;
                        }
                        PlayAutomaticTurn(p2, p1, showNPCsTurn);
                        break;

                    case 1:
                        PlayOneTurn(p1, p2, false);
                        if (!BothFleetsAlive(p1, p2))
                        {
                            break;
                        }
                        PlayAutomaticTurn(p2, p1, false);
                        break;

                    case 2:
                    default:
                        PlayOneTurn(p1, p2);
                        if (!BothFleetsAlive(p1, p2))
                        {
                            break;
                        }
                        PlayOneTurn(p2, p1);
                        break;
                }
            }
            return CheckForWinners(p1, p2);
        }

        private static bool BothFleetsAlive(Player p1, Player p2)
        {
            return p1.FleetStillAlive() && p2.FleetStillAlive();
        }

        private static Player CheckForWinners(Player p1, Player p2)
        {
            if (p1.FleetStillAlive())
            {
                return p1;
            }
            else
            {
                return p2;
            }
        }

        private static void PlayOneTurn(Player player, Player opponent, bool introduce = true)
        {
            if (introduce)
            {
                Console.Clear();
                Console.WriteLine(player.Name + "'s turn. Press any Key");
                Console.ReadKey();
            }

            Tuple<int, int> whereToShoot = GetOrders(player);
            CellType outcome = Shoot(opponent.Map, whereToShoot);
            player.LastMove = whereToShoot;
            player.LastOutcome = outcome;
            player.EnemyMap.PlotOutcome(whereToShoot, outcome);
            SumUpTurn(player, outcome);
        }

        private static void SumUpTurn(Player player, CellType outcome)
        {
            Console.Clear();
            ShowBothMaps(player);
            Console.WriteLine(OutcomeToString(outcome));
            Console.WriteLine("");
            Console.WriteLine("Press a key to change turn");
            Console.ReadKey();
        }

        private static Tuple<int, int> GetOrders(Player player)
        {
            string? order = null;
            while (order == null || NotValidOrder(order))
            {
                Console.Clear();

                if (player.Opponent.LastMove != null)
                {
                    Console.WriteLine("Admiral " + player.Opponent.Name + " shooted at " + StringFromCoordinates(player.Opponent.LastMove));
                    Console.WriteLine(OutcomeToString(player.Opponent.LastOutcome));
                }

                ShowBothMaps(player);
                Console.WriteLine("Your orders, Admiral " + player.Name + "?");
                order = Console.ReadLine();
            }
            return CoordinatesFromString(order);
        }

        public static string OutcomeToString(CellType outcome)
        {
            return outcome switch
            {
                CellType.Hit => "It'a a hit!",
                CellType.Ship => "It'a a hit!",
                CellType.Sunken => "Hit and sunken!",
                CellType.Water => "Missed!",
                _ => "Did you enter right coordinates?",
            };
        }

        private static bool NotValidOrder(string order)
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

        private static Tuple<int, int> CoordinatesFromString(string order)
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

        private static void ShowBothMaps(Player p)
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
                PrintOnMap(highlight, StringFromInt(x), ConsoleColor.Black, ConsoleColor.White);
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