using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace DOANCNPM
{
    public partial class frmHistory : Form
    {
        private string connectionString = @"Data Source=LAPTOP-TUFA15\SQLEXPRESS;Initial Catalog=2048;Integrated Security=True";
        private DataTable dt = new DataTable();
        public string PlayerName { get; set; }

        public frmHistory(string playerName)
        {
            InitializeComponent();
            PlayerName = playerName;
            lblPlayerName.Text = this.PlayerName; // Set the label here, after assigning the value of playerName
            LoadHistory(); // Load history when form loads
                           // Create Close Button
            Button closeButton = new Button();
            closeButton.Text = "Close";
            closeButton.Click += btnClose_Click;
            closeButton.Dock = DockStyle.Bottom;
            this.Controls.Add(closeButton);
            // Create Delete Selected Button
            Button deleteSelectedButton = new Button();
            deleteSelectedButton.Text = "Delete Selected";
            deleteSelectedButton.Click += btnDeleteSelected_Click;
            deleteSelectedButton.Dock = DockStyle.Bottom;
            this.Controls.Add(deleteSelectedButton);
            dgvHistory.CellContentClick += dgvHistory_CellContentClick;
        }

        private void LoadHistory()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM GameHistory WHERE playerName = @playerName";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    adapter.SelectCommand.Parameters.AddWithValue("@playerName", this.PlayerName);

                    dt.Clear();
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        dgvHistory.DataSource = dt;
                    }
                    else
                    {
                        dgvHistory.DataSource = null;
                    }
                    // Check if there are any records in the history and notify if empty
                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("Không có lịch sử trò chơi nào!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải lịch sử: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvHistory_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                // Get the selected row number
                int selectedRow = e.RowIndex;
                // Get the cell value (example)
                string cellValue = dgvHistory.Rows[selectedRow].Cells[e.ColumnIndex].Value?.ToString();
                // Example action: Display the selected cell in a MessageBox
                MessageBox.Show($"You clicked cell at row: {selectedRow}, column {e.ColumnIndex}, value: {cellValue}", "Cell Clicked", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close(); // This will close the form
        }

        private void btnDeleteSelected_Click(object sender, EventArgs e)
        {
            if (dgvHistory.SelectedRows.Count > 0)
            {
                // Get the selected row
                DataGridViewRow selectedRow = dgvHistory.SelectedRows[0];
                // Get the value of the primary key for the selected row (e.g., GameID)
                if (selectedRow.Cells["ID"].Value != null)
                {
                    int gameIDToDelete = (int)selectedRow.Cells["ID"].Value;

                    try
                    {
                        using (SqlConnection conn = new SqlConnection(connectionString))
                        {
                            conn.Open();
                            string query = "DELETE FROM GameHistory WHERE ID = @ID";
                            SqlCommand cmd = new SqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@ID", gameIDToDelete);
                            cmd.ExecuteNonQuery();

                            // Refresh the DataGridView by reloading history
                            LoadHistory();
                            MessageBox.Show("Lịch sử đã chọn đã được xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi xóa lịch sử: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Lỗi: Không có giá trị ID hợp lệ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một hàng để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnDeleteHistory_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM GameHistory"; // SQL query to delete all records
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.ExecuteNonQuery();

                    dt.Clear(); // Clear local DataTable
                    dgvHistory.DataSource = null;
                    MessageBox.Show("Lịch sử trò chơi đã được xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadHistory();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa lịch sử: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}