using OnlineChess.Server.Data;
using OnlineChess.Server.Hubs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ChessDbContext>(options =>
    options.UseOracle
    (builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.MapHub<ChessHub>("/chesshub");
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();

