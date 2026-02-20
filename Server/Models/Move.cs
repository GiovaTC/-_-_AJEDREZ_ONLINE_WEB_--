namespace OnlineChess.Server.Models
{
    public class Move
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public int FromX { get; set; }

        public int FromY { get; set; }

        public string ToX { get; set; }

        public string ToY { get; set; }
    }   
}
