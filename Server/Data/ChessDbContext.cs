using Microsoft.EntityFrameworkCore;
using OnlineChess.Server.Models;

namespace OnlineChess.Server.Data
{
    public class ChessDbContext : DbContext 
    {
        public ChessDbContext(DbContextOptions<ChessDbContext> options) : 
            base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Move> Moves { get; set; }
    }
}   
