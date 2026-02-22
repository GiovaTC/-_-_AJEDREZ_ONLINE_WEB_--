using Microsoft.AspNetCore.SignalR;
using OnlineChess.Server.Data;
using OnlineChess.Shared.Models;

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
            await Groups.AddToGroupAsync(
                Context.ConnectionId, 
                gameId.ToString()
                );
        }
        public async Task MakeMove(int gameId, Move move)
        {
            _db.Moves.Add(move);
            await _db.SaveChangesAsync();

            await Clients.Group(gameId.ToString())
                .SendAsync("MovePlayed", move);
        }
    }
}   
