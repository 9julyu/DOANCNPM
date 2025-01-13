using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace DOANCNPM
{
    public partial class frm2048 : Form
    {
        private Game2048 game;
        private Button BtnHistory;
        private string connectionString = @"Data Source=LAPTOP-TUFA15\SQLEXPRESS;Initial Catalog=2048;Integrated Security=True";

        private string currentPlayerName = "Player 1"; // Tên người chơi mặc định
        private bool IsNewHighScore { get; set; }


        public frm2048()
        {
            InitializeComponent();
            StartNewGame();
            lblPlayerName.Text = $"Player: {currentPlayerName}";

            // Create History Button
            btnHistory = new Button();
            btnHistory.Text = "History";
            btnHistory.Click += BtnHistory_Click;
            btnHistory.Dock = DockStyle.Bottom;
            this.Controls.Add(btnHistory);

            // Thiết lập màu sắc
            btnHistory.BackColor = Color.SteelBlue;       // Màu nền
            btnHistory.ForeColor = Color.White;           // Màu chữ
            btnHistory.Font = new Font("Arial", 12); //font
            btnHistory.FlatStyle = FlatStyle.Flat;     // Kiểu hiển thị phẳng
            btnHistory.FlatAppearance.BorderSize = 0;   // Loại bỏ border của FlatStyle
            btnHistory.FlatAppearance.MouseDownBackColor = Color.LightSkyBlue;
            btnHistory.FlatAppearance.MouseOverBackColor = Color.CornflowerBlue;
        }
        private bool CheckHighScore(int score, string playerName)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT MAX(Score) FROM GameHistory WHERE playerName = @playerName";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@playerName", playerName);
                    object result = cmd.ExecuteScalar();
                    if (result == DBNull.Value) return true;
                    int highScore = Convert.ToInt32(result);
                    return score > highScore;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi kiểm tra điểm cao: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        private void SaveGameToDatabase(int score, string playerName)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi kết nối: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    string query = "INSERT INTO GameHistory (Score, PlayedAt, playerName) VALUES (@Score, @PlayedAt, @playerName)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Score", score);
                    cmd.Parameters.AddWithValue("@PlayedAt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@playerName", playerName);
                    cmd.ExecuteNonQuery();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu lịch sử game: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void StartNewGame()
        {
            game = new Game2048();
            UpdateUI();
        }

        private void UpdateUI()
        {
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    var label = (Label)tlpGameBoard.GetControlFromPosition(col, row);
                    int value = game.Board[row, col];
                    label.Text = value == 0 ? "" : value.ToString();
                    label.BackColor = GetTileColor(value);
                }
            }

            lblScore.Text = $"Score: {game.Score}";
        }

        private Color GetTileColor(int value)
        {
            switch (value)
            {
                case 0: return Color.Beige;
                case 2: return Color.LightYellow;
                case 4: return Color.LightGoldenrodYellow;
                case 8: return Color.Gold;
                case 16: return Color.Orange;
                case 32: return Color.OrangeRed;
                case 64: return Color.Red;
                case 128: return Color.LightGreen;
                case 256: return Color.Green;
                case 512: return Color.DarkGreen;
                case 1024: return Color.Blue;
                case 2048: return Color.Purple;
                default: return Color.Black;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            bool hasMoved = false;

            switch (keyData)
            {
                case Keys.Up: hasMoved = game.MoveUp(); break;
                case Keys.Down: hasMoved = game.MoveDown(); break;
                case Keys.Left: hasMoved = game.MoveLeft(); break;
                case Keys.Right: hasMoved = game.MoveRight(); break;
            }

            if (hasMoved)
            {
                game.AddRandomTile();
                UpdateUI();
                try
                {
                    if (game.IsGameWon())
                    {
                        IsNewHighScore = CheckHighScore(game.Score, currentPlayerName);
                        SaveGameToDatabase(game.Score, currentPlayerName);
                        if (IsNewHighScore)
                        {
                            MessageBox.Show($"Bạn đã thắng! Kỉ lục mới: {game.Score}", "Chúc mừng", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show($"Bạn đã thắng! Điểm số: {game.Score}", "Chúc mừng", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else if (game.IsGameOver())
                    {
                        IsNewHighScore = CheckHighScore(game.Score, currentPlayerName);
                        SaveGameToDatabase(game.Score, currentPlayerName);
                        if (IsNewHighScore)
                        {
                            MessageBox.Show($"Game Over! Kỉ lục mới: {game.Score}", "Kết Thúc Game", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            MessageBox.Show($"Game Over! Điểm số: {game.Score}", "Kết Thúc Game", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi gọi hàm SaveGameToDatabase : {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void BtnNewGame_Click(object sender, EventArgs e)
        {
            StartNewGame();
            MessageBox.Show("Game mới đã được sẵn sàng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnHistory_Click(object sender, EventArgs e)
        {

            frmHistory historyForm = new frmHistory(currentPlayerName);
            historyForm.ShowDialog();
        }
        private void btnChangePlayer_Click(object sender, EventArgs e)
        {
            // 1. Hiển thị hộp thoại nhập tên người chơi
            InputBoxResult result = InputBox("Nhập tên người chơi", "Tên người chơi mới:");

            if (result.OK)
            {
                // 2. Lưu tên người chơi mới
                currentPlayerName = result.Text;
                // 3. Cập nhật giao diện
                lblPlayerName.Text = $"Player: {currentPlayerName}";
            }
        }
        // Function to display input box and get input from the user
        private InputBoxResult InputBox(string title, string promptText)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;


            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;


            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            return new InputBoxResult(dialogResult == DialogResult.OK, textBox.Text);
        }
        // Helper class to represent results from input box
        private class InputBoxResult
        {
            public bool OK { get; private set; }
            public string Text { get; private set; }

            public InputBoxResult(bool ok, string text)
            {
                this.OK = ok;
                this.Text = text;
            }
        }
    }
}