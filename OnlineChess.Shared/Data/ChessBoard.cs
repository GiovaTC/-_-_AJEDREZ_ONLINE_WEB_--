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