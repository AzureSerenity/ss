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
    public partial class Form1 : Form
    {        
        static StudentDBContext context = new StudentDBContext();
        List<Faculty> listFaculties = context.Faculties.ToList();//Lay cac khoa
        List<Student> listStudent = context.Students.ToList();//Lay sinh vien

        public Form1()
        {
            InitializeComponent();
        }

        //Binding list hien thi Ten khoa, gia tri la Ma khoa
        private void FillFacultyCombobox(List<Faculty> listFaculties)
        {
            this.cbbKhoa.DataSource = listFaculties;
            this.cbbKhoa.DisplayMember = "FacultyName";
            this.cbbKhoa.ValueMember = "FacultyID";
        }

        //Binding gridView tu list Student
        private void BindGird(List<Student> listStudent)
        {
            dgvDanhSach.Rows.Clear();
            foreach (var item in listStudent)
            {
                int index = dgvDanhSach.Rows.Add();
                dgvDanhSach.Rows[index].Cells[0].Value = item.StudentID;
                dgvDanhSach.Rows[index].Cells[1].Value = item.FullName;
                dgvDanhSach.Rows[index].Cells[2].Value = item.Faculty.FacultyName;
                dgvDanhSach.Rows[index].Cells[3].Value = item.AverageScore;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                FillFacultyCombobox(listFaculties);
                BindGird(listStudent);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void resetNull()
        {
            txtMaSV.Text = txtHoTen.Text = txtDiemTB.Text = string.Empty;
        }

        public void insertOrUpdate(int row)
        {
            dgvDanhSach.Rows[row].Cells[0].Value = txtMaSV.Text;
            dgvDanhSach.Rows[row].Cells[1].Value = txtHoTen.Text;
            dgvDanhSach.Rows[row].Cells[2].Value = cbbKhoa.Text;
            dgvDanhSach.Rows[row].Cells[3].Value = txtDiemTB.Text;
            //Lui xuong CSDL
            //insert 1 doi tuong sinh vien s vao DB
            Student s = new Student()
            {
                StudentID = int.Parse(txtMaSV.Text),
                FullName = txtHoTen.Text,
                FacultyID = 1,
                AverageScore = decimal.Parse(txtDiemTB.Text)
            };
            context.Students.Add(s);
            context.SaveChanges();
        }

        //Kiem tra cac truong rong
        private bool checkNull()
        {
            if (txtHoTen.Text == "" || txtDiemTB.Text == "" || txtDiemTB.Text == "")
            {
                return true;
            }
            return false;
        }

        //MaSV chi duoc nhap so
        private void txtMaSV_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                MessageBox.Show("Chỉ được nhập số", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //Kiem tra ma sinh vien co hop le chua
        private bool checkMaSV()
        {
            string pattern = @"^\d{10}$";
            return Regex.IsMatch(txtMaSV.Text, pattern);
        }

        //Kiem tra ma sinh vien da ton tai truoc do chua
        private bool checkMaSvExists()
        {
            int.TryParse(txtMaSV.Text, out int maSV);
                return context.Students.Any(s => s.StudentID == maSV);            
        }

        //Kiem tra diem hop le
        private void txtDiemTB_Leave(object sender, EventArgs e)
        {
            double value;
            if (double.TryParse(txtDiemTB.Text, out value))
            {
                if (value < 0 || value > 10)
                {
                    MessageBox.Show("Điểm không nằm trong khoảng từ 0 đến 10!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDiemTB.Focus();
                }
            }

            else
            {
                MessageBox.Show("Giá trị không hợp lệ.");
                txtDiemTB.Focus();
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (checkNull())
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (!checkMaSV())
            {
                MessageBox.Show("Mã sinh viên phải đủ 10 kí tự chữ số!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (checkMaSvExists())
            {
                MessageBox.Show("Mã sinh viên đã tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                int row = dgvDanhSach.Rows.Add();
                insertOrUpdate(row);
                MessageBox.Show("Thêm mới dữ liệu thành công!", "Thông báo");
                resetNull();
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                if(!checkMaSvExists())
                {
                    MessageBox.Show("Không tìm thấy thông tin cần sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    int.TryParse(txtMaSV.Text, out int maSV);

                    Student studentUpdate = context.Students.FirstOrDefault(s => s.StudentID == maSV);

                    if (studentUpdate != null)
                    {
                        studentUpdate.FullName = txtHoTen.Text;
                        studentUpdate.FacultyID = (int)cbbKhoa.SelectedValue;
                        studentUpdate.AverageScore = decimal.Parse(txtDiemTB.Text);

                        context.SaveChanges();
                        BindGird(listStudent);

                        MessageBox.Show("Cập nhật dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            resetNull();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (!checkMaSvExists())
            {
                MessageBox.Show("Không tìm thấy mã sinh viên cần xóa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                DialogResult result = MessageBox.Show("Bạn có chắc muốn xóa sinh viên?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    foreach (DataGridViewRow row in dgvDanhSach.Rows)
                    {
                        if (row.Cells[0].Value != null && row.Cells[0].Value.ToString() == txtMaSV.Text)
                        {
                            dgvDanhSach.Rows.Remove(row);
                            break;
                        }
                    }

                    int.TryParse(txtMaSV.Text, out int maSV);
                    Student deleteStudent = context.Students.FirstOrDefault(s => s.StudentID == maSV);
                    if (deleteStudent != null)
                    {
                        context.Students.Remove(deleteStudent);
                        context.SaveChanges();
                        MessageBox.Show("Xóa sinh viên thành công", "Thông báo", MessageBoxButtons.OK);
                    }
                }
            }
            resetNull();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvDanhSach_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = dgvDanhSach.CurrentCell.RowIndex;
            if (e.RowIndex >= 0)
            {
                txtMaSV.Text = dgvDanhSach.Rows[index].Cells[0].Value.ToString();
                txtHoTen.Text = dgvDanhSach.Rows[index].Cells[1].Value.ToString();
                cbbKhoa.Text = dgvDanhSach.Rows[index].Cells[2].Value.ToString();
                txtDiemTB.Text = dgvDanhSach.Rows[index].Cells[3].Value.ToString();
            }
        }

        private void tsbQuanLyKhoa_Click(object sender, EventArgs e)
        {
            frmFaculty frmFaculty = new frmFaculty();
            frmFaculty.Show();
        }

        private void quảnLýKhoaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmFaculty frmFaculty = new frmFaculty();
            frmFaculty.Show();
        }
    }
}
