namespace OnlineChess.Shared.Models
{
    /// <summary>
    /// Representa un movimiento de ajedrez entre dos casillas
    /// </summary>
    public class Move
    {
        public int FromX { get; set; }
        public int FromY { get; set; }

        public int ToX { get; set; }
        public int ToY { get; set; }
    }
}