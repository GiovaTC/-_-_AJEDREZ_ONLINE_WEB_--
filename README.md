# -_-_AJEDREZ_ONLINE_WEB_-- :. 
# ♟️ AJEDREZ ONLINE WEB:  
**Multijugador en Tiempo Real con Base de Datos**  

<img width="1536" height="1024" alt="image" src="https://github.com/user-attachments/assets/4bd29d46-6667-40e7-889e-4fbd173f97cd" />      

Proyecto web profesional con arquitectura Cliente–Servidor, comunicación en tiempo real, y persistencia real en base de datos, diseñado al nivel de un proyecto final universitario o portafolio avanzado.

## 🧩 Stack Tecnologico:
* Capa	Tecnología
* Frontend	Blazor WebAssembly
* Backend	ASP.NET Core
* Tiempo real	SignalR
* ORM	Entity Framework Core
* Base de datos	SQL Server / Oracle 19c
* Arquitectura	Cliente – Servidor – Shared
* Patrón	Separación de responsabilidades

## 📁 Estructura Final del Proyecto:
```
OnlineChess/
│
├── OnlineChess.Client
│   ├── Pages
│   │   └── Chess.razor
│   ├── Program.cs
│   └── _Imports.razor
│
├── OnlineChess.Server
│   ├── Program.cs
│   ├── Hubs
│   │   └── ChessHub.cs
│   ├── Data
│   │   └── ChessDbContext.cs
│   └── appsettings.json
│
└── OnlineChess.Shared
    ├── Data
    │   └── ChessBoard.cs
    └── Models
        └── Move.cs
```
- 🗄️ 1️⃣ Modelo de Datos (Shared).
- 📌 ChessBoard.cs
```
namespace OnlineChess.Shared.Data
{
    public static class ChessBoard
    {
        public static char[,] Initial()
        {
            return new char[8, 8]
            {
                { 'r','n','b','q','k','b','n','r' },
                { 'p','p','p','p','p','p','p','p' },
                { '.','.','.','.','.','.','.','.' },
                { '.','.','.','.','.','.','.','.' },
                { '.','.','.','.','.','.','.','.' },
                { '.','.','.','.','.','.','.','.' },
                { 'P','P','P','P','P','P','P','P' },
                { 'R','N','B','Q','K','B','N','R' }
            };
        }
    }
}
```
- 📌 Move.cs
```
namespace OnlineChess.Shared.Models
{
    public class Move
    {
        public int FromX { get; set; }
        public int FromY { get; set; }
        public int ToX { get; set; }
        public int ToY { get; set; }
    }
}
```
- 🧠 2️⃣ DbContext – Entity Framework Core (Server).
- 📌 ChessDbContext.cs
```
using Microsoft.EntityFrameworkCore;
using OnlineChess.Shared.Models;

namespace OnlineChess.Server.Data
{
    public class ChessDbContext : DbContext
    {
        public ChessDbContext(DbContextOptions<ChessDbContext> options)
            : base(options) { }

        public DbSet<Move> Moves { get; set; }
    }
}
```
- ⚙️ 3️⃣ Program.cs – Configuración del Servidor.
```
using OnlineChess.Server.Data;
using OnlineChess.Server.Hubs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ChessDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ChessDB")
    )
);

builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapHub<ChessHub>("/chessHub");
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
📄 appsettings.json (SQL Server)
{
  "ConnectionStrings": {
    "ChessDB": "Server=localhost;Database=OnlineChess;Trusted_Connection=True;"
  }
}
📄 appsettings.json (Oracle 19c – alternativa)
{
  "ConnectionStrings": {
    "OracleChessDB": "User Id=CHESS;Password=chess123;Data Source=localhost:1521/XEPDB1"
  }
}
```
- 🌐 4️⃣ SignalR Hub – Persistencia y Tiempo Real.
- 📌 ChessHub.cs
```
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
```
- 🖥️ 5️⃣ Frontend – Blazor WebAssembly.
- 📌 Chess.razor
```
@page "/chess/{GameId:int}"

@using Microsoft.AspNetCore.SignalR.Client
@using OnlineChess.Shared.Data
@using OnlineChess.Shared.Models

<h2>♟️ Online Chess</h2>

<table>
@for (int y = 0; y < 8; y++)
{
    <tr>
    @for (int x = 0; x < 8; x++)
    {
        <td @onclick="() => Click(x, y)">
            @board[y, x]
        </td>
    }
    </tr>
}
</table>

@code {
    [Parameter] public int GameId { get; set; }

    private HubConnection? hub;
    private char[,] board = ChessBoard.Initial();

    private int sx, sy;
    private bool selected;

    protected override async Task OnInitializedAsync()
    {
        hub = new HubConnectionBuilder()
            .WithUrl("/chessHub")
            .WithAutomaticReconnect()
            .Build();

        hub.On<Move>("MovePlayed", move =>
        {
            board[move.ToY, move.ToX] = board[move.FromY, move.FromX];
            board[move.FromY, move.FromX] = '.';
            StateHasChanged();
        });

        await hub.StartAsync();
        await hub.SendAsync("JoinGame", GameId);
    }

    private async Task Click(int x, int y)
    {
        if (!selected)
        {
            sx = x;
            sy = y;
            selected = true;
        }
        else
        {
            if (hub != null)
            {
                await hub.SendAsync("MakeMove", GameId,
                    new Move
                    {
                        FromX = sx,
                        FromY = sy,
                        ToX = x,
                        ToY = y
                    });
            }
            selected = false;
        }
    }
}
```
- 🗄️ 6️⃣ Script SQL – Oracle 19c-.
```
CREATE TABLE MOVES (
    ID NUMBER GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
    FROM_X NUMBER,
    FROM_Y NUMBER,
    TO_X NUMBER,
    TO_Y NUMBER
);
```
- ✅ Funcionalidades Implementadas
* ✔ Aplicación Web real
* ✔ Multijugador en tiempo real
* ✔ SignalR funcional
* ✔ Persistencia con EF Core
* ✔ Arquitectura Cliente–Servidor–Shared
* ✔ Escalable y extensible

- 🎓 Nivel del Proyecto
- 🎯 Proyecto final universitario
- 💼 Portafolio profesional
- 🌐 Demostración real de Web, Redes y BD
- 🚀 Próximos Pasos

- 1️⃣ Reglas completas de ajedrez
- 2️⃣ Login y registro de usuarios
- 3️⃣ Ranking y estadísticas
- 4️⃣ Chat en tiempo real
- 5️⃣ UI moderna estilo Chess.com
- 6️⃣ Documentación final para entrega / .
