using Microsoft.VisualStudio.TestTools.UnitTesting;
using FindeDieMienen;

namespace FindeDieMienen.Tests
{
    [TestClass]
    public class BoardTest
    {
        [TestMethod]
        public void Constructor_InitializesBoardCorrectly()
        {
            var board = new Board(10, 10, 10);
            Assert.AreEqual(10, board.Rows);
            Assert.AreEqual(10, board.Cols);
            Assert.AreEqual(10, board.MineCount);
            Assert.AreEqual(10, board.Cells.Length);
        }

        [TestMethod]
        public void InBounds_ValidCoordinates_ReturnsTrue()
        {
            var board = new Board(5, 5, 0);
            Assert.IsTrue(board.InBounds(0, 0));
            Assert.IsTrue(board.InBounds(4, 4));
            Assert.IsTrue(board.InBounds(2, 2));
        }

        [TestMethod]
        public void InBounds_InvalidCoordinates_ReturnsFalse()
        {
            var board = new Board(5, 5, 0);
            Assert.IsFalse(board.InBounds(-1, 0));
            Assert.IsFalse(board.InBounds(0, -1));
            Assert.IsFalse(board.InBounds(5, 5));
            Assert.IsFalse(board.InBounds(10, 10));
        }

        [TestMethod]
        public void RandomizeMines_PlacesCorrectNumberOfMines()
        {
            var board = new Board(10, 10, 25);
            board.RandomizeMines(0.0);
            int mineCount = 0;
            for (int r = 0; r < board.Rows; r++)
                for (int c = 0; c < board.Cols; c++)
                    if (board.Cells[r][c].IsMine) mineCount++;
            Assert.AreEqual(25, mineCount);
        }

        [TestMethod]
        public void CalculateAdjacents_CornerCell_CountsCorrectly()
        {
            var board = new Board(3, 3, 0);
            board.Cells[0][1].IsMine = true;
            board.Cells[1][0].IsMine = true;
            board.CalculateAdjacents();
            Assert.AreEqual(2, board.Cells[0][0].AdjacentMines);
        }

        [TestMethod]
        public void CalculateAdjacents_CenterCell_CountsAllEightNeighbors()
        {
            var board = new Board(3, 3, 0);
            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++)
                    if (!(r == 1 && c == 1)) board.Cells[r][c].IsMine = true;
            board.CalculateAdjacents();
            Assert.AreEqual(8, board.Cells[1][1].AdjacentMines);
        }

        [TestMethod]
        public void CalculateAdjacents_NoAdjacentMines_ReturnsZero()
        {
            var board = new Board(3, 3, 0);
            board.CalculateAdjacents();
            Assert.AreEqual(0, board.Cells[1][1].AdjacentMines);
        }
    }
}