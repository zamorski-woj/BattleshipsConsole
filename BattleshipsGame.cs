namespace Battleships
{

    public static class BattleshipsGame
    {
        public static Tuple<Player, Player> CreateTwoPlayers(int size = 10)
        {
            return CreateTwoPlayers("player 1", "player 2", size);
        }

        public static Tuple<Player, Player> CreateTwoPlayers(string name1, string name2, int size = 10)
        {
            Player player = new(name1);
            Player player2 = new(name2);
            Tuple<Map, Map> maps = CreateMaps(size, CellType.Water, player, player2);
            Tuple<Map, Map> Enemymaps = CreateMaps(size, CellType.Unknown, player, player2);

            player.Map = maps.Item1;
            player2.Map = maps.Item2;
            player.EnemyMap = Enemymaps.Item1;
            player2.EnemyMap = Enemymaps.Item2;
            player.Opponent = player2;
            player2.Opponent = player;
            player.Fleet.GenerateFleet();
            player2.Fleet.GenerateFleet();
            return new Tuple<Player, Player>(player, player2);
        }

        private static Tuple<Map, Map> CreateMaps(int size, CellType cellType, Player? player1 = null, Player? player2 = null)
        {
            if (size < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            player1 ??= new Player("player 1");
            player2 ??= new Player("player 2");

            Map firstMap = new(player1, size);
            Map secondMap = new(player2, size);
            firstMap.FillWith(cellType);
            secondMap.FillWith(cellType);
            return Tuple.Create(firstMap, secondMap);
        }

        public static CellType Shoot(Map map, Tuple<int, int> coordinates)
        {
            int x = coordinates.Item1;
            int y = coordinates.Item2;
            if (map.IsOnMap(coordinates))
            {
                if (map.Grid[x, y] == CellType.Ship || map.Grid[x, y] == CellType.Hit)
                {
                    map.Grid[x, y] = CellType.Hit;
                    return CheckIfSunken(map, coordinates);
                }
                if (map.Grid[x, y] == CellType.Sunken)
                {
                    return CheckIfSunken(map, coordinates);
                }
                else
                {
                    return CellType.Water;
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(coordinates));
            }
        }

        private static CellType CheckIfSunken(Map map, Tuple<int, int> where)
        {
            Player mapOwner = map.Owner;
            Ship whatGotHit = mapOwner.Fleet.GetShipFromCoordinates(where);
            if (whatGotHit != null)
            {
                return whatGotHit.CheckIfSunken(map);
            }
            else throw new Exception("This ship did not belong to any player");
        }

        public static Ship ShipInRandomPosition(Map map, int length)//Randomize
        {
            if (length < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            Random random = new();
            int xCoordinate, yCoordinate;
            Direction direction;

            do
            {
                xCoordinate = random.Next(0, map.Grid.GetLength(0));
                yCoordinate = random.Next(0, map.Grid.GetLength(1));
                direction = (Direction)random.Next(0, 4);
            }
            while (!map.PlacementManager.CanPlaceShip(xCoordinate, yCoordinate, direction, length));

            Direction heading = direction;
            var mainCoordinate = new Tuple<int, int>(xCoordinate, yCoordinate);
            Ship s = new(length, heading, mainCoordinate);
            map.PlacementManager.PlaceShip(s);
            return s;
        }
    }
}