using System.Collections.Generic;

namespace OnlineChess.Server.Models
{
    public class Game
    {
        public int Id { get; set; }
        public int WhitePlayerId { get; set; }
        public int BlackPlayerId { get; set; }
        public bool WhiteTurn { get; set; } = true;

        public List<Move> Moves { get; set; }
    }
}   
