namespace Battleships
{
    public class ShipPlacementManager
    {
        public Map Map { get; }

        public ShipPlacementManager(Map map)
        {
            Map = map;
        }

        public void PlaceShip(Ship ship)
        {
            if (this.CanPlaceShip(ship))
            {
                List<Tuple<int, int>> shipCoordinates = ship.GetCoordinates();

                foreach (var coord in shipCoordinates)
                {
                    Map.Grid[coord.Item1, coord.Item2] = CellType.Ship;
                }
                Map.Owner?.Fleet.Ships.Add(ship);
            }
        }

        public bool CanPlaceShip(int xCoordinate, int yCoordinate, Direction direction, int length)
        {
            Ship ship = new(length, direction, new Tuple<int, int>(xCoordinate, yCoordinate));
            return this.CanPlaceShip(ship);
        }

        public bool CanPlaceShip(Ship ship)
        {
            List<Tuple<int, int>> shipCoordinates = ship.GetCoordinates();
            return shipCoordinates.All(coord => !Map.IsOccupied(coord));
        }
    }
}