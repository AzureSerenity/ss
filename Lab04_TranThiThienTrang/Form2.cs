using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Lab04_TranThiThienTrang
{
    public partial class frmFaculty : Form
    {
        static StudentDBContext context = new StudentDBContext();
        List<Faculty> listFaculties = context.Faculties.ToList();//Lay cac khoa

        public frmFaculty()
        {
            InitializeComponent();
        }

        private void BindGrid(List<Faculty> listFaculties)
        {
            dgvDanhSachKhoa.Rows.Clear();
            foreach (var item in listFaculties)
            {
                int index = dgvDanhSachKhoa.Rows.Add();
                dgvDanhSachKhoa.Rows[index].Cells[0].Value = item.FacultyID;
                dgvDanhSachKhoa.Rows[index].Cells[1].Value = item.FacultyName;
                dgvDanhSachKhoa.Rows[index].Cells[2].Value = item.TotalProfessor;
            }
        }

        private void frmFaculty_Load(object sender, EventArgs e)
        {
            try
            {
                BindGrid(listFaculties);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool checkNull()
        {
            if (txtMaKhoa.Text == "" || txtTenKhoa.Text == "" || txtTongSoGS.Text == "")
            {
                return true;
            }
            return false;
        }

        public void resetNull()
        {
            txtMaKhoa.Text = txtMaKhoa.Text = txtTongSoGS.Text = string.Empty;
        }

        private bool checkMaKhoaExists()
        {
            int.TryParse(txtMaKhoa.Text, out int maKhoa);
            return context.Faculties.Any(s => s.FacultyID == maKhoa);
        }

        private void txtMaKhoa_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                MessageBox.Show("Chỉ được nhập số", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void txtTongSoGS_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                MessageBox.Show("Chỉ được nhập số", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void txtTongSoGS_Leave(object sender, EventArgs e)
        {
            double value;
            if (double.TryParse(txtTongSoGS.Text, out value))
            {
                if (value < 1 || value > 300)
{
                    MessageBox.Show("Giá trị không nằm trong khoảng từ 0 đến 300.");
                    txtTongSoGS.Focus(); 
                }
            }

            else
            {
                MessageBox.Show("Giá trị không hợp lệ.");
                txtTongSoGS.Focus(); 
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (checkNull())
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int facultyID = int.Parse(txtMaKhoa.Text);
                Faculty existingFaculty = context.Faculties.Find(facultyID);

                if (existingFaculty == null)
                {
                    Faculty newFaculty = new Faculty()
                    {
                        FacultyID = facultyID,
                        FacultyName = txtTenKhoa.Text,
                        TotalProfessor = int.Parse(txtTongSoGS.Text),
                    };
                    context.Faculties.Add(newFaculty);
                    context.SaveChanges();
                    MessageBox.Show("Thêm mới thành công.", "Thông báo", MessageBoxButtons.OK);
                }
                else
                {
                    existingFaculty.FacultyName = txtMaKhoa.Text;
                    existingFaculty.TotalProfessor = int.Parse(txtTongSoGS.Text);
                    context.SaveChanges();
                    MessageBox.Show("Cập nhật thành công.", "Thông báo", MessageBoxButtons.OK);
                }

                resetNull();
                BindGrid(context.Faculties.ToList());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (!checkMaKhoaExists())
            {
                MessageBox.Show("Không tìm thấy mã khoa cần xóa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                DialogResult result = MessageBox.Show("Bạn có chắc muốn xóa khoa này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    foreach (DataGridViewRow row in dgvDanhSachKhoa.Rows)
                    {
                        if (row.Cells[0].Value != null && row.Cells[0].Value.ToString() == txtMaKhoa.Text)
                        {
                            dgvDanhSachKhoa.Rows.Remove(row);
                            break;
                        }
                    }

                    int.TryParse(txtMaKhoa.Text, out int maKhoa);
                    Faculty deleteFaculty = context.Faculties.FirstOrDefault(s => s.FacultyID == maKhoa);
                    if (deleteFaculty != null)
                    {
                        context.Faculties.Remove(deleteFaculty);
                        context.SaveChanges();
                        MessageBox.Show("Xóa khoa thành công", "Thông báo", MessageBoxButtons.OK);
                    }
                }
            }
        }

        private void btnDong_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvDanhSachKhoa_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvDanhSachKhoa.Rows[e.RowIndex];
                txtMaKhoa.Text = Convert.ToString(row.Cells[0].Value);
                txtTenKhoa.Text = Convert.ToString(row.Cells[1].Value);
                txtTongSoGS.Text = Convert.ToString(row.Cells[2].Value);
            }
        }
    }
}
