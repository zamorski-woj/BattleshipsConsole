namespace Battleships
{
    public class Ship
    {
        public int Length { get; }
        public Direction Heading { get; }
        public Tuple<int, int> MainCoordinate { get; }
        public bool Sunken { get; private set; } = false;

        public Ship(int length, Direction heading, Tuple<int, int> mainCoordinate)//manually
        {
            this.Length = length;
            this.Heading = heading;
            this.MainCoordinate = mainCoordinate;
        }

        public List<Tuple<int, int>> GetCoordinates()
        {
            List<Tuple<int, int>> allCoordinates = new();
            int xCoordinate = this.MainCoordinate.Item1, yCoordinate = this.MainCoordinate.Item2;
            switch (this.Heading)
            {
                case Direction.North:
                    for (int i = 0; i < this.Length; i++)
                    {
                        allCoordinates.Add(new Tuple<int, int>(xCoordinate, yCoordinate + i));
                    }
                    break;

                case Direction.East:
                    for (int i = 0; i < this.Length; i++)
                    {
                        allCoordinates.Add(new Tuple<int, int>(xCoordinate + i, yCoordinate));
                    }
                    break;

                case Direction.South:
                    for (int i = 0; i < this.Length; i++)
                    {
                        allCoordinates.Add(new Tuple<int, int>(xCoordinate, yCoordinate - i));
                    }
                    break;

                case Direction.West:
                    for (int i = 0; i < this.Length; i++)
                    {
                        allCoordinates.Add(new Tuple<int, int>(xCoordinate - i, yCoordinate));
                    }
                    break;
            }
            return allCoordinates;
        }

        public void Destroy(Map map)
        {
            var coordinates = this.GetCoordinates();
            foreach (var coordinate in coordinates)
            {
                map.Grid[coordinate.Item1, coordinate.Item2] = CellType.Sunken;
            }
            this.Sunken = true;
        }

        internal bool ExistsHere(Tuple<int, int> where)
        {
            var coordinates = GetCoordinates();
            return coordinates.Any(coordinate => coordinate.Item1 == where.Item1 && coordinate.Item2 == where.Item2);
        }

        internal CellType CheckIfSunken(Map map)
        {
            var coordinates = GetCoordinates();
            if (IsAnyCellAlive(map, coordinates))
            {
                return CellType.Hit;//alive
            }
            this.Destroy(map);
            this.Destroy(map.Owner.Opponent.EnemyMap);
            return CellType.Sunken;//He`s dead, Jim.
        }

        internal static bool IsAnyCellAlive(Map map, List<Tuple<int, int>> coordinates)
        {
            return coordinates.Any(coordinate => map.Grid[coordinate.Item1, coordinate.Item2] == CellType.Ship);
        }
    }
}