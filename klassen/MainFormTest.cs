using System;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FindeDieMienen.Tests
{
    [TestClass]
    public class MainFormTest
    {
        static void RunInSta(Action action)
        {
            Exception? ex = null;
            var t = new Thread(() =>
            {
                try { action(); }
                catch (Exception e) { ex = e; }
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
            if (ex != null) throw new AggregateException("Exception in STA thread", ex);
        }

        [TestMethod]
        public void StartGame_CreatesBoard_WithExpectedDimensionsAndMines_Easy()
        {
            RunInSta(() =>
            {
                var form = new MainForm();

                // set rows/cols to 9
                var nudRows = (NumericUpDown)form.GetType().GetField("nudRows", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(form)!;
                var nudCols = (NumericUpDown)form.GetType().GetField("nudCols", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(form)!;
                var cbDiff = (ComboBox)form.GetType().GetField("cbDifficulty", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(form)!;
                nudRows.Value = 9; nudCols.Value = 9; cbDiff.SelectedIndex = 0; // Easy

                // invoke StartGame (private)
                form.GetType().GetMethod("StartGame", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(form, null);

                var board = (object)form.GetType().GetField("board", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(form)!;
                var rows = (int)board.GetType().GetProperty("Rows")!.GetValue(board)!;
                var cols = (int)board.GetType().GetProperty("Cols")!.GetValue(board)!;
                var mineCount = (int)board.GetType().GetProperty("MineCount")!.GetValue(board)!;

                Assert.AreEqual(9, rows);
                Assert.AreEqual(9, cols);
                int area = rows * cols;
                int expected = Math.Max(1, area / 10);
                Assert.AreEqual(expected, mineCount);
            });
        }

        [TestMethod]
        public void StartGame_WithMultiplayer_EntersPlacementModeAndSetsMinesToPlace()
        {
            RunInSta(() =>
            {
                var form = new MainForm();
                var cbMult = (CheckBox)form.GetType().GetField("cbMultiplayer", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(form)!;
                var nudRows = (NumericUpDown)form.GetType().GetField("nudRows", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(form)!;
                var nudCols = (NumericUpDown)form.GetType().GetField("nudCols", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(form)!;
                var cbDiff = (ComboBox)form.GetType().GetField("cbDifficulty", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(form)!;

                nudRows.Value = 8; nudCols.Value = 8; cbDiff.SelectedIndex = 1; // Medium
                cbMult.Checked = true;

                form.GetType().GetMethod("StartGame", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(form, null);

                var placementMode = (bool)form.GetType().GetField("placementMode", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(form)!;
                var minesToPlace = (int)form.GetType().GetField("minesToPlace", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(form)!;

                Assert.IsTrue(placementMode);
                int area = 8 * 8; int expectedMines = Math.Max(1, area / 6);
                Assert.AreEqual(expectedMines, minesToPlace);
            });
        }

        [TestMethod]
        public void FloodReveal_RevealsAllCells_WhenNoMines()
        {
            RunInSta(() =>
            {
                var form = new MainForm();

                // create a board with 5x5 and 0 mines
                var board = new Board(5, 5, 0);
                board.CalculateAdjacents();

                // set private board field
                form.GetType().GetField("board", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(form, board);

                // Build grid (private) so FloodReveal logic is consistent with UI setup
                form.GetType().GetMethod("BuildGrid", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(form, null);

                // call FloodReveal on (0,0)
                form.GetType().GetMethod("FloodReveal", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(form, new object[] { 0, 0 });

                // assert all cells are revealed
                for (int r = 0; r < board.Rows; r++)
                    for (int c = 0; c < board.Cols; c++)
                        Assert.IsTrue(board.Cells[r][c].Revealed, $"Cell {r},{c} was not revealed");
            });
        }

        [TestMethod]
        public void CellClicked_RevealsMine_DecrementsLives()
        {
            RunInSta(() =>
            {
                var form = new MainForm();
                var board = new Board(3, 3, 0);
                board.Cells[1][1].IsMine = true;
                board.CalculateAdjacents();
                form.GetType().GetField("board", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(form, board);
                form.GetType().GetMethod("BuildGrid", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(form, null);

                // Simulate click on mine
                form.GetType().GetMethod("CellClicked", BindingFlags.NonPublic | BindingFlags.Instance)!
                    .Invoke(form, new object[] { 1, 1, MouseButtons.Left });

                var lives = (int)form.GetType().GetField("lives", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(form)!;
                Assert.AreEqual(2, lives);
                Assert.IsTrue(board.Cells[1][1].Revealed);
            });
        }

        [TestMethod]
        public void CellClicked_RightClick_MarksCell()
        {
            RunInSta(() =>
            {
                var form = new MainForm();
                var board = new Board(2, 2, 0);
                board.CalculateAdjacents();
                form.GetType().GetField("board", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(form, board);
                form.GetType().GetMethod("BuildGrid", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(form, null);

                // Simulate right click on (0,0)
                form.GetType().GetMethod("CellClicked", BindingFlags.NonPublic | BindingFlags.Instance)!
                    .Invoke(form, new object[] { 0, 0, MouseButtons.Right });

                Assert.IsTrue(board.Cells[0][0].Marked);
            });
        } 

        // Integrationstest: Simuliert ein komplettes Spiel mit Sieg
        [TestMethod]
        public void Integration_WinGame_AllMinesMarked()
        {
            RunInSta(() =>
            {
                var form = new MainForm();
                var nudRows = (NumericUpDown)form.GetType().GetField("nudRows", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(form)!;
                var nudCols = (NumericUpDown)form.GetType().GetField("nudCols", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(form)!;
                var cbDiff = (ComboBox)form.GetType().GetField("cbDifficulty", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(form)!;
                nudRows.Value = 5; nudCols.Value = 5; cbDiff.SelectedIndex = 2; // Hard

                form.GetType().GetMethod("StartGame", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(form, null);

                var board = (Board)form.GetType().GetField("board", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(form)!;
                // Markiere alle Minen
                for (int r = 0; r < board.Rows; r++)
                    for (int c = 0; c < board.Cols; c++)
                        if (board.Cells[r][c].IsMine)
                            form.GetType().GetMethod("CellClicked", BindingFlags.NonPublic | BindingFlags.Instance)!
                                .Invoke(form, new object[] { r, c, MouseButtons.Right });

                // Prüfe ob Spiel als gewonnen erkannt wird
                var win = (bool)form.GetType().GetMethod("CheckWin", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(form, null)!;
                Assert.IsTrue(win);
            });
        }
    }
}

