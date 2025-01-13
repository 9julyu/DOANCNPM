using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DOANCNPM
{
    public partial class frmChangePlayer : Form
    {
        public string PlayerName { get; private set; }
        public frmChangePlayer()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem tên người chơi có bị bỏ trống không
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Tên không được để trống, vui lòng nhập lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtName.Focus(); // Di chuyển con trỏ chuột vào lại textbox để người dùng nhập lại
                return; // Dừng việc đóng form
            }
            this.PlayerName = txtName.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();   
        }
    }
}
