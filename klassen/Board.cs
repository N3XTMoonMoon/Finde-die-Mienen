using System;

namespace FindeDieMienen
{
    public class Board
    {
        public int Rows { get; set; }
        public int Cols { get; set; }
        public int MineCount { get; set; }
        public Cell[][] Cells { get; set; }

        public Board() { }
        public Board(int rows, int cols, int mines)
        {
            Rows = rows; Cols = cols; MineCount = mines;
            Cells = new Cell[rows][];
            for (int r = 0; r < rows; r++) { Cells[r] = new Cell[cols]; for (int c = 0; c < cols; c++) Cells[r][c] = new Cell(); }
        }

        public bool InBounds(int r, int c) => r >= 0 && c >= 0 && r < Rows && c < Cols;

        public void RandomizeMines(double clusterBias)
        {
            var rand = new Random(); int placed = 0;
            while (placed < MineCount)
            {
                int r = rand.Next(Rows), c = rand.Next(Cols);
                if (!Cells[r][c].IsMine) { Cells[r][c].IsMine = true; placed++; }
            }
            CalculateAdjacents();
        }

        public void CalculateAdjacents()
        {
            for (int r = 0; r < Rows; r++) for (int c = 0; c < Cols; c++)
            {
                int cnt = 0;
                for (int dr = -1; dr <= 1; dr++) for (int dc = -1; dc <= 1; dc++)
                    if (!(dr == 0 && dc == 0) && InBounds(r + dr, c + dc) && Cells[r + dr][c + dc].IsMine) cnt++;
                Cells[r][c].AdjacentMines = cnt;
            }
        }
    }
}