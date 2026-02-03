namespace FindeDieMienen
{
    public class SaveState
    {
        public int Rows { get; set; }
        public int Cols { get; set; }
        public int Lives { get; set; }
        public int MineCount { get; set; }
        public Cell[][] Cells { get; set; } = null!;
        public int RemainingSeconds { get; set; }
        public bool Multiplayer { get; set; }
    }
}