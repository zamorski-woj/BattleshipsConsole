namespace Battleships
{
    
    public class Map
    {
        public CellType[,] Grid { get; }
        public Player Owner { get; }
        public ShipPlacementManager PlacementManager { get; }

        public Map(Player p, int size)
        {
            Grid = new CellType[size, size];
            Owner = p;
            PlacementManager = new(this);
        }

        public Map(int size = 10)
        {
            Grid = new CellType[size, size];
            Owner = new Player("Ziutek");
            PlacementManager = new(this);
        }

        public void FillWith(CellType cellType)
        {
            for (int x = 0; x < (int)Math.Sqrt(Grid.Length); x++)
            {
                for (int y = 0; y < (int)Math.Sqrt(Grid.Length); y++)
                {
                    Grid.SetValue(cellType, x, y);
                }
            }
        }

        public bool IsOnMap(Tuple<int, int> coordinates)
        {
            int mapSize = (int)Math.Sqrt(this.Grid.Length);
            int xCoordinate = coordinates.Item1;
            int yCoordinate = coordinates.Item2;

            if (xCoordinate < 0 || yCoordinate < 0 || xCoordinate >= mapSize || yCoordinate >= mapSize)
            {
                return false;
            }
            return true;
        }

        public bool IsOccupied(Tuple<int, int> coord)
        {
            return this.IsOccupied(coord.Item1, coord.Item2);
        }

        public bool IsOccupied(int x, int y)
        {
            if (!IsOnMap(new Tuple<int, int>(x, y)))
            {
                return true;
            }
            else
            {
                return (Grid[x, y] != CellType.Water && Grid[x, y] != CellType.Unknown);
            }
        }

        internal void PlotOutcome(Tuple<int, int> whereToShoot, CellType outcome)
        {
            Grid[whereToShoot.Item1, whereToShoot.Item2] = outcome;
        }
    }
}