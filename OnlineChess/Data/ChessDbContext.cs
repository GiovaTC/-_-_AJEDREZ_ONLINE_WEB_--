using Microsoft.EntityFrameworkCore;
using OnlineChess.Shared.Models;

namespace OnlineChess.Server.Data
{
    public class ChessDbContext : DbContext 
    {
        public ChessDbContext(DbContextOptions<ChessDbContext> options) : 
            base(options) { }

     //   public DbSet<Player> Players { get; set; }
     //   public DbSet<GameState> Games { get; set; }
        public DbSet<Move> Moves { get; set; }
    }
}   
