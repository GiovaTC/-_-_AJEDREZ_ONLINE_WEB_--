# -_-_AJEDREZ_ONLINE_WEB_-- :. 
# ♟️ AJEDREZ ONLINE WEB:  
**Multijugador en Tiempo Real con Base de Datos**  

<img width="1536" height="1024" alt="image" src="https://github.com/user-attachments/assets/4bd29d46-6667-40e7-889e-4fbd173f97cd" />  

Proyecto **web profesional** con arquitectura **Cliente–Servidor**, persistencia real y comunicación en tiempo real, diseñado al nivel de un **proyecto final universitario** o **portafolio avanzado**.

---

## 🧩 Stack Tecnológico:

| Capa | Tecnología |
|---|---|
| **Frontend** | Blazor Server |
| **Backend** | ASP.NET Core |
| **Tiempo real** | SignalR |
| **ORM** | Entity Framework Core |
| **Base de datos** | SQL Server / Oracle 19c |
| **Autenticación** | Usuarios simples |
| **Arquitectura** | Cliente–Servidor |

---

## 📁 Estructura Final del Proyecto:

```text
OnlineChess/
│
├── Server/
│   ├── Program.cs
│   ├── Data/
│   │   ├── ChessDbContext.cs
│   │   └── DbInitializer.cs
│   ├── Hubs/
│   │   └── ChessHub.cs
│   ├── Models/
│   │   ├── User.cs
│   │   ├── Game.cs
│   │   └── Move.cs
│
├── Client/
│   └── Pages/
│       └── Chess.razor
│
└── Shared/
    └── ChessBoard.cs

🗄️ 1️⃣ Base de Datos – Modelo de Entidades.
📌 User.cs
public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
}
📌 Game.cs
using System.Collections.Generic;

public class Game
{
    public int Id { get; set; }
    public int WhitePlayerId { get; set; }
    public int BlackPlayerId { get; set; }
    public bool WhiteTurn { get; set; } = true;

    public List<Move> Moves { get; set; }
}
📌 Move.cs
public class Move
{
    public int Id { get; set; }
    public int GameId { get; set; }

    public int FromX { get; set; }
    public int FromY { get; set; }
    public int ToX { get; set; }
    public int ToY { get; set; }
}

🧠 2️⃣ DbContext – Entity Framework Core.
using Microsoft.EntityFrameworkCore;

public class ChessDbContext : DbContext
{
    public ChessDbContext(DbContextOptions<ChessDbContext> options)
        : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<Move> Moves { get; set; }
}

⚙️ 3️⃣ Program.cs – Configuración de BD y SignalR.
▶ SQL Server
builder.Services.AddDbContext<ChessDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ChessDB")));
▶ Oracle 19c
builder.Services.AddDbContext<ChessDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleChessDB")));
builder.Services.AddSignalR();
📄 appsettings.json (SQL Server)
{
  "ConnectionStrings": {
    "ChessDB": "Server=localhost;Database=OnlineChess;Trusted_Connection=True;"
  }
}
📄 appsettings.json (Oracle)
{
  "ConnectionStrings": {
    "OracleChessDB": "User Id=CHESS;Password=chess123;Data Source=localhost:1521/XEPDB1"
  }
}

🌐 4️⃣ SignalR Hub – Persistencia de Partidas.
📌 ChessHub.cs
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

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
}

🖥️ 5️⃣ Frontend – Blazor (Chess.razor).
@page "/chess/{GameId:int}"
@using Microsoft.AspNetCore.SignalR.Client

<h2>♟️ Online Chess</h2>

<table>
@for (int y = 0; y < 8; y++)
{
    <tr>
    @for (int x = 0; x < 8; x++)
    {
        <td @onclick="() => Click(x,y)">
            @board[y, x]
        </td>
    }
    </tr>
}
</table>

@code {
    [Parameter] public int GameId { get; set; }

    HubConnection hub;
    char[,] board = ChessBoard.Initial();

    int sx, sy;
    bool selected;

    protected override async Task OnInitializedAsync()
    {
        hub = new HubConnectionBuilder()
            .WithUrl("/chessHub")
            .Build();

        hub.On<Move>("MovePlayed", m =>
        {
            board[m.ToY, m.ToX] = board[m.FromY, m.FromX];
            board[m.FromY, m.FromX] = '.';
            StateHasChanged();
        });

        await hub.StartAsync();
        await hub.SendAsync("JoinGame", GameId);
    }

    async Task Click(int x, int y)
    {
        if (!selected)
        {
            sx = x;
            sy = y;
            selected = true;
        }
        else
        {
            await hub.SendAsync("MakeMove", GameId,
                new Move { FromX = sx, FromY = sy, ToX = x, ToY = y });

            selected = false;
        }
    }
}

🗄️ 6️⃣ Script SQL – Oracle 19c.
CREATE TABLE USERS (
    ID NUMBER GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
    USERNAME VARCHAR2(50) UNIQUE
);

CREATE TABLE GAMES (
    ID NUMBER GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
    WHITE_PLAYER_ID NUMBER,
    BLACK_PLAYER_ID NUMBER,
    WHITE_TURN NUMBER(1)
);

CREATE TABLE MOVES (
    ID NUMBER GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
    GAME_ID NUMBER,
    FROM_X NUMBER,
    FROM_Y NUMBER,
    TO_X NUMBER,
    TO_Y NUMBER,
    FOREIGN KEY (GAME_ID) REFERENCES GAMES(ID)
);

✅ Funcionalidades Implementadas
* ✔ Aplicación Web
* ✔ Multijugador real
* ✔ Tiempo real con SignalR
* ✔ Persistencia en Base de Datos
* ✔ Registro de partidas y movimientos
* ✔ Arquitectura Cliente–Servidor
* ✔ Escalable y extensible.

* 🎓 Nivel del Proyecto
* 🎯 Proyecto final universitario
* 💼 Portafolio profesional
* 🌐 Demostración real de redes, web y bases de datos
* 🚀 Próximos Pasos (Extensiones).

* 1️⃣ Reglas completas de ajedrez
* 2️⃣ Login / Registro de usuarios
* 3️⃣ Ranking y estadísticas
* 4️⃣ Chat en tiempo real
* 5️⃣ UI moderna estilo Chess.com
* 6️⃣ Documentación final lista para entrega / :. / . .
