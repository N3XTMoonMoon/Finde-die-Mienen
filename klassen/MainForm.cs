using System;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace FindeDieMienen
{
    public class MainForm : Form
    {
        NumericUpDown nudRows, nudCols;
        ComboBox cbDifficulty;
        CheckBox cbMultiplayer;
        Button btnStart, btnSave, btnLoad, btnReset;
        Label lblLives, lblTimer;
        Panel gridPanel;
        System.Windows.Forms.Timer gameTimer;

        Board board = null!;
        Button[][] buttons = null!;
        int lives = 3;
        TimeSpan timeLimit = TimeSpan.Zero;
        DateTime endTime;
        bool placementMode = false;
        int minesToPlace = 0;

        public MainForm()
        {
            Text = "Finde-die-Mienen";
            Width = 900; Height = 700;
            InitializeComponents();
        }

        void InitializeComponents()
        {
            var top = new FlowLayoutPanel { AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Padding = new Padding(12), WrapContents = true, Dock = DockStyle.Fill };

            top.Controls.Add(new Label { Text = "Rows:", AutoSize = true });
            nudRows = new NumericUpDown { Minimum = 5, Maximum = 30, Value = 9, Width = 60 };
            top.Controls.Add(nudRows);
            top.Controls.Add(new Label { Text = "Cols:", AutoSize = true });
            nudCols = new NumericUpDown { Minimum = 5, Maximum = 30, Value = 9, Width = 60 };
            top.Controls.Add(nudCols);

            top.Controls.Add(new Label { Text = "Difficulty:", AutoSize = true });
            cbDifficulty = new ComboBox { Width = 100, DropDownStyle = ComboBoxStyle.DropDownList };
            cbDifficulty.Items.AddRange(new[] { "Easy", "Medium", "Hard" }); cbDifficulty.SelectedIndex = 0;
            top.Controls.Add(cbDifficulty);

            cbMultiplayer = new CheckBox { Text = "Multiplayer (manuell platzieren)", AutoSize = true };
            top.Controls.Add(cbMultiplayer);

            btnStart = new Button { Text = "Start", AutoSize = true };
            btnStart.Click += (s, e) => StartGame();
            top.Controls.Add(btnStart);

            btnSave = new Button { Text = "Save", AutoSize = true };
            btnSave.Click += (s, e) => SaveGame(); top.Controls.Add(btnSave);
            btnLoad = new Button { Text = "Load", AutoSize = true };
            btnLoad.Click += (s, e) => LoadGame(); top.Controls.Add(btnLoad);
            btnReset = new Button { Text = "Reset", AutoSize = true };
            btnReset.Click += (s, e) => ResetUI(); top.Controls.Add(btnReset);

            lblLives = new Label { Text = "Leben: 3", AutoSize = true, Margin = new Padding(20, 6, 6, 6) };
            top.Controls.Add(lblLives);
            lblTimer = new Label { Text = "Zeit: ∞", AutoSize = true, Margin = new Padding(6) };
            top.Controls.Add(lblTimer);

            gridPanel = new Panel { Dock = DockStyle.Fill, AutoScroll = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Padding = new Padding(6, 12, 6, 12) };

            var mainLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            mainLayout.Controls.Add(top, 0, 0);
            mainLayout.Controls.Add(gridPanel, 0, 1);
            Controls.Add(mainLayout);

            gameTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            gameTimer.Tick += GameTimer_Tick;
        }

        void StartGame()
        {
            int rows = (int)nudRows.Value; int cols = (int)nudCols.Value;
            var diff = cbDifficulty.SelectedIndex; // 0 easy,1 med,2 hard
            var settings = DifficultySettings(diff, rows, cols);
            lives = 3; lblLives.Text = $"Leben: {lives}";
            timeLimit = TimeSpan.FromSeconds(settings.timeSeconds);
            if (timeLimit.TotalSeconds > 0) endTime = DateTime.Now + timeLimit;
            else endTime = DateTime.MaxValue;
            lblTimer.Text = timeLimit.TotalSeconds > 0 ? $"Zeit: {timeLimit}" : "Zeit: ∞";

            board = new Board(rows, cols, settings.mineCount);
            if (cbMultiplayer.Checked)
            {
                placementMode = true;
                minesToPlace = settings.mineCount;
                MessageBox.Show($"Platzierenmodus: Setze {minesToPlace} Minen durch Linksklicks.");
            }
            else
            {
                board.RandomizeMines(settings.clusterBias);
                placementMode = false;
            }

            BuildGrid();
            gameTimer.Start();
        }

        void BuildGrid()
        {
            gridPanel.Controls.Clear();
            int rows = board.Rows, cols = board.Cols;
            buttons = new Button[rows][];
            var table = new TableLayoutPanel { ColumnCount = cols, RowCount = rows, AutoSize = true, CellBorderStyle = TableLayoutPanelCellBorderStyle.Single };
            table.Margin = new Padding(6, 12, 6, 6);
            for (int c = 0; c < cols; c++) table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            for (int r = 0; r < rows; r++) table.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            for (int r = 0; r < rows; r++)
            {
                buttons[r] = new Button[cols];
                for (int c = 0; c < cols; c++)
                {
                    var b = new Button { Width = 40, Height = 40, Margin = new Padding(0) };
                    int rr = r, cc = c;
                    b.MouseUp += (s, e) => CellClicked(rr, cc, e.Button);
                    buttons[r][c] = b;
                    table.Controls.Add(b, c, r);
                }
            }
            gridPanel.Controls.Add(table);
            RefreshGrid();
        }

        void CellClicked(int r, int c, MouseButtons button)
        {
            if (board == null) return;
            var cell = board.Cells[r][c];
            if (placementMode)
            {
                if (!cell.IsMine && minesToPlace > 0) { cell.IsMine = true; minesToPlace--; board.CalculateAdjacents(); }
                if (minesToPlace == 0) { placementMode = false; MessageBox.Show("Platzierung abgeschlossen."); }
                RefreshGrid();
                return;
            }

            if (button == MouseButtons.Right)
            {
                if (!cell.Revealed) cell.Marked = !cell.Marked;
            }
            else
            {
                if (cell.IsMine)
                {
                    cell.Revealed = true; lives--; lblLives.Text = $"Leben: {lives}";
                    board.MineCount--;
                    if (lives <= 0) { RevealAll(); MessageBox.Show("Verloren — keine Leben mehr."); gameTimer.Stop(); }
                }
                else
                {
                    FloodReveal(r, c);
                }
            }
            if (CheckWin()) { RevealAll(); MessageBox.Show("Gewonnen — alle Minen markiert!\nGlückwunsch!"); gameTimer.Stop(); }
            RefreshGrid();
        }

        void RefreshGrid()
        {
            if (buttons == null) return;
            for (int r = 0; r < board.Rows; r++) for (int c = 0; c < board.Cols; c++)
                {
                    var b = buttons[r][c]; var cell = board.Cells[r][c];
                    if (cell.Revealed)
                    {
                        b.Enabled = false;
                        b.Text = cell.IsMine ? "💣" : (cell.AdjacentMines == 0 ? "" : "" + cell.AdjacentMines);
                        b.BackColor = Color.LightGray;
                    }
                    else if (cell.Marked) { b.Text = "⚑"; b.BackColor = Color.LightYellow; }
                    else { b.Text = ""; b.BackColor = SystemColors.Control; b.Enabled = true; }
                }
        }

        void FloodReveal(int r, int c)
        {
            if (!board.InBounds(r, c)) return;
            var cell = board.Cells[r][c];
            if (cell.Revealed || cell.Marked) return;
            cell.Revealed = true;
            if (cell.AdjacentMines > 0) return;
            for (int dr = -1; dr <= 1; dr++) for (int dc = -1; dc <= 1; dc++) if (!(dr == 0 && dc == 0)) FloodReveal(r + dr, c + dc);
        }

        bool CheckWin()
        {
            int marked = 0;
            for (int r = 0; r < board.Rows; r++) for (int c = 0; c < board.Cols; c++) if (board.Cells[r][c].Marked && board.Cells[r][c].IsMine) marked++;
            return marked >= board.MineCount;
        }

        void RevealAll()
        {
            for (int r = 0; r < board.Rows; r++) for (int c = 0; c < board.Cols; c++) board.Cells[r][c].Revealed = true;
            RefreshGrid();
        }

        void GameTimer_Tick(object sender, EventArgs e)
        {
            if (timeLimit.TotalSeconds <= 0) { lblTimer.Text = "Zeit: ∞"; return; }
            var rem = endTime - DateTime.Now;
            if (rem <= TimeSpan.Zero)
            {
                lblTimer.Text = "Zeit: 00:00:00";
                gameTimer.Stop(); RevealAll(); MessageBox.Show("Zeit abgelaufen — verloren.");
            }
            else lblTimer.Text = $"Zeit: {rem.ToString(@"hh\:mm\:ss")}";
        }

        void SaveGame()
        {
            if (board == null) { MessageBox.Show("Kein Spiel aktiv"); return; }
            using var sfd = new SaveFileDialog { Filter = "JSON|*.json", FileName = "save.json" };
            if (sfd.ShowDialog() != DialogResult.OK) return;
            var state = new SaveState { Rows = board.Rows, Cols = board.Cols, Lives = lives, MineCount = board.MineCount, Cells = board.Cells, RemainingSeconds = (int)Math.Max(0,(endTime - DateTime.Now).TotalSeconds), Multiplayer = placementMode };
            var opts = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(sfd.FileName, JsonSerializer.Serialize(state, opts));
            MessageBox.Show("Gespeichert.");
        }

        void LoadGame()
        {
            using var ofd = new OpenFileDialog { Filter = "JSON|*.json" };
            if (ofd.ShowDialog() != DialogResult.OK) return;
            try
            {
                var json = File.ReadAllText(ofd.FileName);
                var state = JsonSerializer.Deserialize<SaveState>(json);
                if (state == null) return;
                board = new Board(state.Rows, state.Cols, state.MineCount);
                board.Cells = state.Cells ?? board.Cells;
                board.CalculateAdjacents();
                lives = state.Lives; lblLives.Text = $"Leben: {lives}";
                timeLimit = TimeSpan.FromSeconds(state.RemainingSeconds);
                endTime = DateTime.Now + timeLimit;
                placementMode = state.Multiplayer;
                BuildGrid();
                gameTimer.Start();
            }
            catch (Exception ex) { MessageBox.Show("Fehler beim Laden: " + ex.Message); }
        }

        void ResetUI()
        {
            gridPanel.Controls.Clear();
            buttons = null!; board = null!; gameTimer.Stop(); lblTimer.Text = "Zeit: ∞"; lblLives.Text = "Leben: 3";
        }

        static (int mineCount, int timeSeconds, double clusterBias) DifficultySettings(int idx, int rows, int cols)
        {
            int area = rows * cols;
            return idx switch
            {
                0 => (Math.Max(1, area / 10), 600, 0.0),
                1 => (Math.Max(1, area / 6), 300, 0.35),
                2 => (Math.Max(1, area / 4), 180, 0.6),
                _ => (Math.Max(1, area / 10), 600, 0.0)
            };
        }
    }
}
