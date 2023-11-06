using static Battleships.BattleshipsGame;

namespace Battleships
{
    public class Fleet
    {
        public List<Ship> Ships { get; } = new();
        public Player Owner { get; }

        public Fleet(Player owner)
        {
            this.Owner = owner;
        }

        public void GenerateFleet(List<int>? list = null)
        {
            if (list == null || list.Count < 1)
            {
                list = new List<int>() { 5, 4, 4, 3, 3, 3, 2, 2 };
            }

            foreach (int i in list)
            {
                ShipInRandomPosition(Owner.Map, i);
            }
        }

        public Ship? GetShipFromCoordinates(Tuple<int, int> where)
        {
            return Ships.FirstOrDefault(ship => ship.ExistsHere(where));
        }

        public bool StillAlive()
        {
            return Ships.Any(ship => ship.Sunken == false);
        }
    }
}