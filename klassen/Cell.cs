namespace FindeDieMienen
{
    public class Cell
    {
        public bool IsMine { get; set; }
        public bool Revealed { get; set; }
        public bool Marked { get; set; }
        public int AdjacentMines { get; set; }
    }
}