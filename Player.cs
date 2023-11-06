namespace Battleships
{
    public class Player
    {
        public Fleet Fleet { get; }
        public string Name { get; }
        public Map Map { get; internal set; }
        public Map EnemyMap { get; internal set; }
        public Player Opponent { get; internal set; }
        public Tuple<int, int>? LastMove { get; internal set; }
        public CellType LastOutcome { get; internal set; }

        public Player(string v = "Ziutek")
        {
            this.Name = v;
            this.Fleet = new(this);
        }
        
    }
}