﻿using static Battleships.BattleshipsConsole;
using static Battleships.BattleshipsGame;

namespace Battleships
{
    public static class NPC
    {
        public static Tuple<int, int> PlayAutomaticTurn(Player player, Player opponent, bool showNPC = true)
        {
            Tuple<int, int> whereToShoot = ChooseWhereToShoot(player.EnemyMap);
            CellType outcome = Shoot(opponent.Map, whereToShoot);
            player.EnemyMap.PlotOutcome(whereToShoot, outcome);
            player.LastMove = whereToShoot;
            player.LastOutcome = outcome;

            if (showNPC)
            {
                Console.Clear();
                Console.WriteLine("Admiral " + player.Name + " shoots at " + Coordinates.StringFromCoordinates(whereToShoot));
                Console.WriteLine(OutcomeToString(outcome));
                Console.WriteLine();
                MapPrinter.ShowMap(player.EnemyMap, whereToShoot);
                Console.ReadKey();
                Console.Clear();
            }
            return whereToShoot;
        }

        private static Tuple<int, int> ChooseWhereToShoot(Map enemyMap)
        {
            Random r = new();
            int mapSize = enemyMap.Grid.GetLength(0);
            double[,] weight = new double[mapSize, mapSize];

            for (int y = 0; y < mapSize; y++)
            {
                for (int x = 0; x < mapSize; x++)
                {
                    weight[x, y] = r.NextDouble();//fill with random small numbers, so they are not equal but negligable
                }
            }
            for (int y = 0; y < mapSize; y++)
            {
                for (int x = 0; x < mapSize; x++)
                {
                    switch (enemyMap.Grid[x, y])
                    {
                        case CellType.Unknown:
                            weight[x, y] += 1;
                            break;

                        case CellType.Hit:
                            weight[x, y] -= 999;
                            weight = ChangeNeighbouring(weight, x, y, +50);
                            PredictLines(enemyMap, weight, x, y);
                            break;

                        case CellType.Ship://how would he know?
                            weight[x, y] += 999;
                            weight = ChangeNeighbouring(weight, x, y, +50);
                            break;

                        case CellType.Sunken:
                            weight[x, y] -= 999;
                            weight = ChangeNeighbouring(weight, x, y, -5);
                            break;

                        default:
                        case CellType.Water:
                            weight[x, y] -= 999;
                            break;
                    }
                }
            }

            double maxValue = weight.Cast<double>().Max();
            return IndexOf(weight, maxValue);
        }

        private static void PredictLines(Map map, double[,] weight, int hitX, int hitY)//we know xy was hit
        {
            CheckHorizontalLines(map, weight, hitX, hitY, 1);
            CheckHorizontalLines(map, weight, hitX, hitY, -1);
            CheckVerticalLines(map, weight, hitX, hitY, 1);
            CheckVerticalLines(map, weight, hitX, hitY, -1);
        }

        private static void CheckVerticalLines(Map map, double[,] weight, int hitX, int hitY, int increase)
        {
            int size = map.Grid.GetLength(0);
            bool endloop = false;//lets me break out of loop inside the switch. ambigous "break" keyword
            for (int y = hitY + increase; y < size && y >= 0; y++)
            {
                if (endloop)
                {
                    break;
                }
                switch (map.Grid[hitX, y])
                {
                    case CellType.Hit:
                        break;

                    case CellType.Unknown:
                        weight[hitX, y] += 100;
                        endloop = true;
                        break;

                    default:
                        endloop = true;
                        break;
                }
            }
        }

        private static void CheckHorizontalLines(Map map, double[,] weight, int hitX, int hitY, int increase)
        {
            int size = map.Grid.GetLength(0);
            bool endloop = false;//lets me break out of loop inside the switch. ambigous "break" keyword
            for (int x = hitX + increase; x < size && x >= 0; x++)
            {
                if (endloop)
                {
                    break;
                }
                switch (map.Grid[x, hitY])
                {
                    case CellType.Hit:
                        break;

                    case CellType.Unknown:
                        weight[x, hitY] += 100;
                        endloop = true;
                        break;

                    default:
                        endloop = true;
                        break;
                }
            }
        }

        private static Tuple<int, int> IndexOf(double[,] array, double value)
        {
            int size = array.GetLength(0);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    if (array[x, y] == value)
                    {
                        return new Tuple<int, int>(x, y);
                    }
                }
            }
            return new Tuple<int, int>(0, 0);
        }

        public static double[,] ChangeNeighbouring(double[,] array, int x, int y, int howMuch)
        {
            if (x + 1 < array.GetLength(0))
            {
                array[x + 1, y] += howMuch;
            }
            if (y + 1 < array.GetLength(1))
            {
                array[x, y + 1] += howMuch;
            }
            if (x - 1 >= 0)
            {
                array[x - 1, y] += howMuch;
            }
            if (y - 1 >= 0)
            {
                array[x, y - 1] += howMuch;
            }
            return array;
        }
    }
}