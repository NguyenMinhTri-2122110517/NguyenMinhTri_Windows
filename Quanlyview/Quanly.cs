﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing; // Ensure you have this using directive for Image
using System.Data.SqlClient;

namespace Quanlyview
{
    public partial class Quanly : Form
    {
        private string strCon = @"Data Source=LAPTOP-L9MO5U5F;Initial Catalog=Employee;User ID=sa;Password=123;Encrypt=False;";
        private SqlConnection sqlCon; // Khai báo SqlConnection

        public List<Employee> lstEmp = new List<Employee>();
        private BindingSource bs = new BindingSource();
        public bool isThoat = true;
        public event EventHandler DangXuat;

        private string employeeImagePath = string.Empty; // Store the image path

        public Quanly()
        {
            InitializeComponent();
            SetupImageList();

            tbSearch.TextChanged += tbSearch_TextChanged;

            //ngay sinh
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "dd MMMM yyyy";
            // Handle value changes (optional)
            dateTimePicker1.ShowUpDown = false;
        }

        private void Quanly_Load(object sender, EventArgs e)
        {
            lstEmp = GetData();
            bs.DataSource = lstEmp;
            dgvEmployee.DataSource = bs;
            SetupDataGridView(); // Setup DataGridView columns
            dateTimePicker1.Value = DateTime.Now; // Set the default date to now
            dgvEmployee.EditMode = DataGridViewEditMode.EditProgrammatically;

        }

        public List<Employee> GetData()
        {
            List<Employee> employee = new List<Employee>();

            using (sqlCon = new SqlConnection(strCon)) // Sử dụng từ khóa using để quản lý tài nguyên
            {
                sqlCon.Open(); // Mở kết nối

                // Câu truy vấn để lấy dữ liệu
                string query = "SELECT MaSV, TenSV, NgaySinh, GioiTinh, DiaChi, SoDienThoai, Email, MaLop, NganhHoc, ImagePath FROM Employee";

                using (SqlCommand cmd = new SqlCommand(query, sqlCon)) // Tạo SqlCommand
                {
                    using (SqlDataReader reader = cmd.ExecuteReader()) // Sử dụng using cho SqlDataReader
                    {
                        while (reader.Read()) // Đọc dữ liệu
                        {
                            Employee emp = new Employee
                            {
                                MaSV = reader.GetInt32(0), // Mã
                                TenSV = reader.GetString(1), // Tên
                                NgaySinh = reader.GetDateTime(2), // Ngày sinh
                                GioiTinh = reader.GetBoolean(3), // Giới tính
                                DiaChi = reader.GetString(4), // Địa chỉ
                                SoDienThoai = reader.GetString(5),
                                Email = reader.GetString(6),
                                MaLop = reader.GetString(7),
                                NganhHoc = reader.GetString(8),
                                ImagePath = reader.IsDBNull(9) ? null : reader.GetString(9) // Ảnh
                            };
                            employee.Add(emp); // Thêm vào danh sách
                        }
                    }
                }
            }
            return employee; // Trả về danh sách nhân viên
        }
        private void AddEmployee(Employee newEmp)
        {
            using (sqlCon = new SqlConnection(strCon))
            {
                sqlCon.Open();
                string query = "INSERT INTO Employee (MaSV, TenSV, NgaySinh, GioiTinh, DiaChi, SoDienThoai, Email, MaLop, NganhHoc, ImagePath) VALUES (@MaSV, @TenSV, @NgaySinh, @GioiTinh, @DiaChi, @SoDienThoai, @Email, @MaLop, @NganhHoc, @ImagePath)";

                using (SqlCommand cmd = new SqlCommand(query, sqlCon))
                {
                    cmd.Parameters.AddWithValue("@MaSV", newEmp.MaSV);
                    cmd.Parameters.AddWithValue("@TenSV", newEmp.TenSV);
                    cmd.Parameters.AddWithValue("@NgaySinh", newEmp.NgaySinh);
                    cmd.Parameters.AddWithValue("@GioiTinh", newEmp.GioiTinh);
                    cmd.Parameters.AddWithValue("@DiaChi", newEmp.DiaChi);
                    cmd.Parameters.AddWithValue("@SoDienThoai", newEmp.SoDienThoai);
                    cmd.Parameters.AddWithValue("@Email", newEmp.Email);
                    cmd.Parameters.AddWithValue("@MaLop", newEmp.MaLop);
                    cmd.Parameters.AddWithValue("@NganhHoc", newEmp.NganhHoc);
                    cmd.Parameters.AddWithValue("@ImagePath", newEmp.ImagePath);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        private void UpdateEmployee(Employee emp)
        {
            using (sqlCon = new SqlConnection(strCon))
            {
                sqlCon.Open();
                string query = "UPDATE Employee SET TenSV=@TenSV, NgaySinh=@NgaySinh, GioiTinh=@GioiTinh, DiaChi=@DiaChi, SoDienThoai=@SoDienThoai, Email=@Email, MaLop=@MaLop, NganhHoc=@NganhHoc, ImagePath=@ImagePath WHERE MaSV=@MaSV";

                using (SqlCommand cmd = new SqlCommand(query, sqlCon))
                {
                    cmd.Parameters.AddWithValue("@MaSV", emp.MaSV);
                    cmd.Parameters.AddWithValue("@TenSV", emp.TenSV);
                    cmd.Parameters.AddWithValue("@NgaySinh", emp.NgaySinh);
                    cmd.Parameters.AddWithValue("@GioiTinh", emp.GioiTinh);
                    cmd.Parameters.AddWithValue("@DiaChi", emp.DiaChi);
                    cmd.Parameters.AddWithValue("@SoDienThoai", emp.SoDienThoai);
                    cmd.Parameters.AddWithValue("@Email", emp.Email);
                    cmd.Parameters.AddWithValue("@MaLop", emp.MaLop);
                    cmd.Parameters.AddWithValue("@NganhHoc", emp.NganhHoc);
                    cmd.Parameters.AddWithValue("@ImagePath", emp.ImagePath);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        private void DeleteEmployee(int empId)
        {
            using (sqlCon = new SqlConnection(strCon))
            {
                sqlCon.Open();
                string query = "DELETE FROM Employee WHERE MaSV=@MaSV";

                using (SqlCommand cmd = new SqlCommand(query, sqlCon))
                {
                    cmd.Parameters.AddWithValue("@MaSV", empId);
                    cmd.ExecuteNonQuery();
                }
            }
        }



        private void SetupDataGridView()
        {
            dgvEmployee.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvEmployee.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvEmployee.Columns[0].HeaderText = "Mã sv";
            dgvEmployee.Columns[1].HeaderText = "Tên sv";
            dgvEmployee.Columns[2].HeaderText = "Ngày Sinh";
            dgvEmployee.Columns[3].HeaderText = "Giới Tính";
            dgvEmployee.Columns[4].HeaderText = "Địa Chỉ";
            dgvEmployee.Columns[5].HeaderText = "Số Điện Thoại";
            dgvEmployee.Columns[6].HeaderText = "Email";
            dgvEmployee.Columns[7].HeaderText = "Mã Lớp";
            dgvEmployee.Columns[7].HeaderText = "Ngành Học";
            dgvEmployee.Columns[8].HeaderText = "Ảnh"; // Add header for Birth Date
        }

        private void btThoat_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btDangXuat_Click(object sender, EventArgs e)
        {
            DangXuat?.Invoke(this, EventArgs.Empty);
        }

        private void Quanly_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (isThoat) Application.Exit();
        }

        // Add this method for email and phone validation
        private bool IsValidEmail(string email)
        {
            return email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsValidPhoneNumber(string phone)
        {
            return phone.All(char.IsDigit);
        }

        private void btAddNew_Click(object sender, EventArgs e)
        {
            try
            {
                tbId.Enabled = true; // Đảm bảo mở khóa ô ID khi thêm mới

                // Kiểm tra từng trường dữ liệu có hợp lệ không
                if (string.IsNullOrWhiteSpace(tbId.Text))
                {
                    MessageBox.Show("Lỗi: Vui lòng nhập mã sinh viên.");
                    return;
                }

                if (tbName.Text.Any(ch => !char.IsLetter(ch) && !char.IsWhiteSpace(ch)))
                {
                    MessageBox.Show("Lỗi: Tên sinh viên không được chứa số hoặc ký tự đặc biệt.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(tbName.Text))
                {
                    MessageBox.Show("Lỗi: Vui lòng nhập tên sinh viên.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(tbAddress.Text))
                {
                    MessageBox.Show("Lỗi: Vui lòng nhập địa chỉ.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(tbPhone.Text) || !IsValidPhoneNumber(tbPhone.Text))
                {
                    MessageBox.Show("Lỗi: Số điện thoại không hợp lệ. Vui lòng chỉ nhập số.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(tbEmail.Text) || !IsValidEmail(tbEmail.Text))
                {
                    MessageBox.Show("Lỗi: Email phải có đuôi @gmail.com.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(cbMalop.Text))
                {
                    MessageBox.Show("Lỗi: Vui lòng chọn mã lớp.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(cbNganhhoc.Text))
                {
                    MessageBox.Show("Lỗi: Vui lòng chọn ngành học.");
                    return;
                }

                if (string.IsNullOrEmpty(employeeImagePath))
                {
                    MessageBox.Show("Lỗi: Vui lòng chọn một ảnh.");
                    return;
                }

                // Kiểm tra ID có trùng lặp không
                int newId = int.Parse(tbId.Text);

                if (newId <= 0) // Check if ID is less than or equal to 0
                {
                    MessageBox.Show("Lỗi: Mã sinh viên phải lớn hơn 0.");
                    return;
                }

                if (lstEmp.Any(emp => emp.MaSV == newId))
                {
                    MessageBox.Show("Lỗi: ID đã tồn tại. Vui lòng nhập ID khác.");
                    return;
                }

                // Tạo đối tượng Employee mới
                var newEmp = new Employee
                {
                    MaSV = newId,
                    TenSV = tbName.Text,
                    GioiTinh = ckGender.Checked,
                    DiaChi = tbAddress.Text,
                    SoDienThoai = tbPhone.Text,
                    Email = tbEmail.Text,
                    MaLop = cbMalop.Text,
                    NganhHoc = cbNganhhoc.Text,
                    ImagePath = employeeImagePath,
                    NgaySinh = dateTimePicker1.Value.Date
                };

                // Thêm vào danh sách và cập nhật DataGridView
                lstEmp.Add(newEmp);
                AddEmployee(newEmp);
                RefreshBindings();
                ClearInputFields(); // Xóa dữ liệu các ô nhập sau khi thêm xong

                tbId.Enabled = true; // Đảm bảo mở lại ô ID cho lần thêm tiếp theo
            }
            catch (FormatException)
            {
                MessageBox.Show("Lỗi: Vui lòng nhập số nguyên hợp lệ cho ID.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message);
            }
        }

        private void btEdit_Click(object sender, EventArgs e)
        {
            if (dgvEmployee.CurrentRow == null) return;

            var idx = dgvEmployee.CurrentRow.Index;
            var emp = lstEmp[idx];
            int oldMaSV = emp.MaSV;

            try
            {
                int newId = int.Parse(tbId.Text);

                if (oldMaSV != newId)
                {
                    MessageBox.Show("Lỗi: Không thể sửa đổi mã sinh viên.");
                    tbId.Text = oldMaSV.ToString();
                    return;
                }

                if (!IsValidPhoneNumber(tbPhone.Text))
                {
                    MessageBox.Show("Lỗi: Số điện thoại không hợp lệ. Vui lòng chỉ nhập số.");
                    return;
                }

                if (!IsValidEmail(tbEmail.Text))
                {
                    MessageBox.Show("Lỗi: Email phải có đuôi @gmail.com.");
                    return;
                }

                if (tbName.Text.Any(ch => !char.IsLetter(ch) && !char.IsWhiteSpace(ch)))
                {
                    MessageBox.Show("Lỗi: Tên sinh viên không được chứa số hoặc ký tự đặc biệt.");
                    return;
                }

                emp.TenSV = tbName.Text;
                emp.GioiTinh = ckGender.Checked;
                emp.DiaChi = tbAddress.Text;
                emp.SoDienThoai = tbPhone.Text;
                emp.Email = tbEmail.Text;
                emp.MaLop = cbMalop.Text;
                emp.NganhHoc = cbNganhhoc.Text;
                emp.ImagePath = employeeImagePath;
                emp.NgaySinh = dateTimePicker1.Value.Date;

                RefreshBindings();
                UpdateEmployee(emp);
                ClearInputFields();
            }
            catch (FormatException)
            {
                MessageBox.Show("Lỗi: Vui lòng nhập số nguyên hợp lệ cho ID.");
            }
        }




        private void btDelete_Click(object sender, EventArgs e)
        {
            if (dgvEmployee.CurrentRow == null) return;

            int idx = dgvEmployee.CurrentRow.Index;
            var empId = lstEmp[idx].MaSV;

            lstEmp.RemoveAt(idx);
            DeleteEmployee(empId);
            RefreshBindings();
        }

        private void dgvEmployee_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= lstEmp.Count) return;

            var emp = lstEmp[e.RowIndex];

            tbId.Text = emp.MaSV.ToString();
            tbName.Text = emp.TenSV;
            ckGender.Checked = emp.GioiTinh;
            tbAddress.Text = emp.DiaChi;
            tbPhone.Text = emp.SoDienThoai;
            tbEmail.Text = emp.Email;
            cbMalop.Text = emp.MaLop;
            cbNganhhoc.Text = emp.NganhHoc;

            // Set the date of birth, or use the current date if none is available
            dateTimePicker1.Value = (emp.NgaySinh != DateTime.MinValue) ? emp.NgaySinh : DateTime.Now;

            if (File.Exists(emp.ImagePath))
            {
                pbEmployeeImage.Image = Image.FromFile(emp.ImagePath);
            }
            else
            {
                pbEmployeeImage.Image = null;
            }
        }

        //private void dgvManager_CellContentClick(object sender, DataGridViewCellEventArgs e)
        //{
        //    if (e.RowIndex < 0 || e.RowIndex >= lstEmp.Count) return;

        //    var emp = lstEmp[e.RowIndex];

        //    tbId.Text = emp.MaSV.ToString();
        //    tbName.Text = emp.TenSV;
        //    tbPhone.Text = emp.SoDienThoai;
        //    tbEmail.Text = emp.Email;
        //    tbAddress.Text = emp.DiaChi;
        //    cbMalop.Text = emp.MaLop;
        //    cbNganhhoc.Text = emp.NganhHoc;
        //    ckGender.Checked = emp.GioiTinh;
        //    if (emp.NgaySinh != null && emp.NgaySinh >= dateTimePicker1.MinDate && emp.NgaySinh <= dateTimePicker1.MaxDate)
        //    {
        //        dateTimePicker1.Value = emp.NgaySinh;
        //    }
        //    else
        //    {
        //        dateTimePicker1.Value = DateTime.Now; // Default to current date
        //    }


        //    if (File.Exists(emp.ImagePath))
        //    {
        //        pbEmployeeImage.Image = Image.FromFile(emp.ImagePath);
        //    }
        //    else
        //    {
        //        pbEmployeeImage.Image = null;
        //    }
        //}

        private void RefreshBindings()
        {
            bs.DataSource = lstEmp.ToList();
            bs.ResetBindings(false);
            dgvEmployee.ClearSelection(); // Optional: Clear selection for better UX
        }


        private void ClearInputFields()
        {
            tbId.Clear();
            tbName.Clear();
            tbAddress.Clear();
            tbPhone.Clear();
            tbEmail.Clear();
            cbMalop.SelectedIndex = -1; // or clear
            cbNganhhoc.SelectedIndex = -1; // or clear
            ckGender.Checked = false;
            dateTimePicker1.Value = DateTime.Now;

            if (pbEmployeeImage.Image != null)
            {
                pbEmployeeImage.Image.Dispose();
                pbEmployeeImage.Image = null; // Clear the image
            }

            employeeImagePath = string.Empty; // Reset the image path
        }


        private void SetupImageList()
        {
            ImageList imageList = new ImageList();
            imageList.ImageSize = new Size(24, 24);

            // Add images to ImageList (make sure paths are correct)
            imageList.Images.Add(Image.FromFile("Images/add.png"));    // Index 0
            imageList.Images.Add(Image.FromFile("Images/edit.png"));   // Index 1
            imageList.Images.Add(Image.FromFile("Images/delete.png")); // Index 2

            // Assign ImageList to buttons
            btAddNew.ImageList = imageList;
            btAddNew.ImageIndex = 0;

            btEdit.ImageList = imageList;
            btEdit.ImageIndex = 1;

            btDelete.ImageList = imageList;
            btDelete.ImageIndex = 2;
        }

        private void btSelectImage_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem tên đã được nhập hay chưa
            if (string.IsNullOrWhiteSpace(tbName.Text))
            {
                MessageBox.Show("Lỗi: Vui lòng nhập tên trước khi chọn ảnh.");
                return;
            }

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Image files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    employeeImagePath = openFileDialog.FileName; // Store the selected image path
                    pbEmployeeImage.Image = Image.FromFile(employeeImagePath); // Display the image
                    pbEmployeeImage.SizeMode = PictureBoxSizeMode.StretchImage;
                }
            }
        }


        // Method to set a specific date for the DateTimePicker (if needed)
        private void SetDateForDateTimePicker(DateTime date)
        {
            dateTimePicker1.Value = date; // Set a specific date, e.g. new DateTime(2024, 10, 17)
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            DateTime selectedDate = dateTimePicker1.Value;
            // Do something with the selected date
            this.Text = dateTimePicker1.Value.ToString("dd MMMM yyyy");
        }

        private void dgvEmployee_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void tbPhone_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbAddress_TextChanged(object sender, EventArgs e)
        {

        }
        private void tbSearch_TextChanged(object sender, EventArgs e)
        {
            string searchTerm = tbSearch.Text.ToLower();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                bs.DataSource = lstEmp; // Nếu ô tìm kiếm trống, hiển thị danh sách đầy đủ
            }
            else
            {
                var filteredList = lstEmp.Where(emp =>
                    emp.MaSV.ToString().Contains(searchTerm) || // Tìm theo mã sinh viên (chuyển sang chuỗi)
                    emp.TenSV.ToLower().Contains(searchTerm) ||
                    emp.DiaChi.ToLower().Contains(searchTerm) ||
                    emp.SoDienThoai.Contains(searchTerm) || // Tìm theo số điện thoại
                    emp.Email.ToLower().Contains(searchTerm) ||
                    emp.MaLop.ToLower().Contains(searchTerm) ||
                    emp.NganhHoc.ToLower().Contains(searchTerm)
                ).ToList();

                bs.DataSource = filteredList; // Cập nhật BindingSource với kết quả tìm kiếm
            }

            bs.ResetBindings(false); // Làm mới DataGridView để hiển thị kết quả lọc
        }

    }
}
