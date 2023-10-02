using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab04_TranThiThienTrang
{
    public partial class TimKiemSV : Form
    {
        static StudentDBContext context = new StudentDBContext();
        List<Student> listStudent = context.Students.ToList();
        List<Faculty> listFaculties = context.Faculties.ToList();
        private List<Student> searchResults = new List<Student>();
        private int totalSearchResults = 0;

        public TimKiemSV()
        {
            InitializeComponent();
        }

        private void FillFalcultyCombobox(List<Faculty> listFaculties)
        {
            if (listFaculties != null && listFaculties.Any())
            {
                this.cmbFaculty.DataSource = listFaculties;
                this.cmbFaculty.DisplayMember = "FacultyName";
                this.cmbFaculty.ValueMember = "FacultyID";
            }
            else
            {
                // Hiển thị thông báo hoặc xử lý tùy ý nếu danh sách rỗng.
            }
        }

        private void BindGrid(List<Student> listStudent)
        {
            dgvTimKiem.Rows.Clear();
            foreach (var item in listStudent)
            {
                int index = dgvTimKiem.Rows.Add();
                dgvTimKiem.Rows[index].Cells[0].Value = item.StudentID;
                dgvTimKiem.Rows[index].Cells[1].Value = item.FullName;
                dgvTimKiem.Rows[index].Cells[2].Value = item.Faculty.FacultyName;
                dgvTimKiem.Rows[index].Cells[3].Value = item.AverageScore;
            }
        }

        public void resetNull()
        {
            txtFullName.Text = txtStudentID.Text = string.Empty;
            cmbFaculty.SelectedIndex = 0;
        }

        public bool checkNull()
        {
            if (txtStudentID.Text == "" || txtFullName.Text == "")
            {
                return true;
            }
            return false;
        }



        private void btnSearch_Click_1(object sender, EventArgs e)
        {
            string studentID = txtStudentID.Text.Trim().ToLower();
            string fullName = txtFullName.Text.Trim().ToLower();
            int facultyID = (int)cmbFaculty.SelectedValue;

            try
            {
                var query = from student in listStudent
                            where
                                (string.IsNullOrEmpty(studentID) || student.StudentID.ToString().Contains(studentID)) &&
                                (string.IsNullOrEmpty(fullName) || student.FullName.ToLower().Contains(fullName)) &&
                                (facultyID == 0 || student.FacultyID == facultyID)
                            select student;

                List<Student> newSearchResults = query.ToList();

                if (newSearchResults.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy sinh viên nào trong danh sách.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    foreach (var student in newSearchResults)
                    {
                        bool existsInGrid = dgvTimKiem.Rows.Cast<DataGridViewRow>().Any(row =>
                            row.Cells[0].Value.ToString().ToLower() == student.StudentID.ToString() ||
                            row.Cells[1].Value.ToString().ToLower() == student.FullName.ToLower());

                        if (existsInGrid)
                        {
                            MessageBox.Show("Sinh viên đã tồn tại trong danh sách.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            int index = dgvTimKiem.Rows.Add();
                            dgvTimKiem.Rows[index].Cells[0].Value = student.StudentID;
                            dgvTimKiem.Rows[index].Cells[1].Value = student.FullName;
                            dgvTimKiem.Rows[index].Cells[2].Value = student.Faculty.FacultyName;
                            dgvTimKiem.Rows[index].Cells[3].Value = student.AverageScore;

                            totalSearchResults += 1;
                        }
                    }

                    txtSearchResults.Text = totalSearchResults.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi tìm kiếm sinh viên: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnComeBack_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            dgvTimKiem.Rows.Clear();
            searchResults.Clear();
            totalSearchResults = 0;
            txtSearchResults.Text = searchResults.Count.ToString();
        }

        private void TimKiemSV_Load(object sender, EventArgs e)
        {
            FillFalcultyCombobox(listFaculties);
        }
    }
}

