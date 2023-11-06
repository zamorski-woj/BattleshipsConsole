using static Battleships.BattleshipsGame;
using static Battleships.NPC;

namespace Battleships
{
    public static class BattleshipsConsole
    {
        public static void Menu()
        {
            int numberOfPlayers = GetNumberOfPlayers();
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

        private static int GetNumberOfPlayers()
        {
            int number = 2;
            string? decision = null;
            while (decision == null)
            {
                Console.WriteLine("How many human players?");
                Console.WriteLine("0 - computer vs computer");
                Console.WriteLine("1 - single player");
                Console.WriteLine("2 - multi player (hot seat)");

                decision = Console.ReadKey().KeyChar.ToString();
                try
                {
                    number = int.Parse(decision);
                }
                catch (FormatException f)
                {
                    continue;//with default 2
                }
            }
            return number;
        }

        public static Player RunGame(int numberOfPlayers, bool showNPCsTurn = true, string name1 = "Ewaryst", string name2 = "Antyfilidor")
        {
            Tuple<Player, Player> bothPlayers = CreateTwoPlayers(name1, name2, 10);
            Player p1 = bothPlayers.Item1;
            Player p2 = bothPlayers.Item2;

            while (BothFleetsAlive(p1, p2))
            {
                PlayTwoTurns(p1, p2, showNPCsTurn, numberOfPlayers);
            }
            return CheckForWinners(p1, p2);
        }

        private static void PlayTwoTurns(Player p1, Player p2, bool showNPCsTurn, int numberOfPlayers)
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

        private static bool BothFleetsAlive(Player p1, Player p2)
        {
            return p1.Fleet.StillAlive() && p2.Fleet.StillAlive();
        }

        private static Player CheckForWinners(Player p1, Player p2)
        {
            if (p1.Fleet.StillAlive())
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

            Tuple<int, int> whereToShoot = GetCoordinatesFromUser(player);
            CellType outcome = Shoot(opponent.Map, whereToShoot);
            player.LastMove = whereToShoot;
            player.LastOutcome = outcome;
            player.EnemyMap.PlotOutcome(whereToShoot, outcome);
            SumUpTurn(player, outcome);
        }

        private static void SumUpTurn(Player player, CellType outcome)
        {
            Console.Clear();
            MapPrinter.ShowBothMaps(player);
            Console.WriteLine(OutcomeToString(outcome));
            Console.WriteLine("");
            Console.WriteLine("Press a key to change turn");
            Console.ReadKey();
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
        public static Tuple<int, int> GetCoordinatesFromUser(Player player)
        {
            string? order = null;
            while (order == null || Coordinates.NotValidOrder(order))
            {
                Console.Clear();
                ShowOpponentLastMove(player);
                MapPrinter.ShowBothMaps(player);
                Console.WriteLine("Your orders, Admiral " + player.Name + "?");
                order = Console.ReadLine();
            }
            return Coordinates.CoordinatesFromString(order);
        }

        private static void ShowOpponentLastMove(Player player)
        {
            if (player.Opponent.LastMove != null)
            {
                Console.WriteLine("Admiral " + player.Opponent.Name + " shooted at " + Coordinates.StringFromCoordinates(player.Opponent.LastMove));
                Console.WriteLine(OutcomeToString(player.Opponent.LastOutcome));
            }
        }
    }
}