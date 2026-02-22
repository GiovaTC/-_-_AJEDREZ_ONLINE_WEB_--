using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineChess.Shared.Models
{
    public class GameState
    {
        public object Moves;

        public GameState(object moves)
        {
            Moves = moves;
        }

        public int Id;
    }
}
