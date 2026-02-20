using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OnlineChess.Server.Data;
using OnlineChess.Server.Models;

namespace OnlineChess.Server.Hubs
{
    public class ChessHub : Hub
    {
        private readonly ChessDbContext _db;
        public ChessHub(ChessDbContext db)
        {
            _db = db;
        }
        public async Task JoinGame(int gameId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId.ToString());

            var game = _db.Games
                .Include(g => g.Moves)
                .First(g => g.Id == gameId);

            await Clients.Caller.SendAsync("GameState", game);
        }
        public async Task MakeMove(int gameId, Move move)
        {
            _db.Moves.Add(move);
            await _db.SaveChangesAsync();

            await Clients.Group(gameId.ToString())
                .SendAsync("MovePlayed", move);
        }
        public async Task LeaveGame(string gameId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
        }
    }
}
