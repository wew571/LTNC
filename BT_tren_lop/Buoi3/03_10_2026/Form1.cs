using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WinFormsApp2
{
    public partial class QLSV : Form
    {
        // In-memory model
        private readonly List<Student> students = new();
        private readonly List<ClassModel> classes = new();

        // Regex for class code: CSE + 4 digits
        private static readonly Regex ClassCodeRegex = new("^CSE\\d{4}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public QLSV()
        {
            InitializeComponent();

            // Wire up button events here so Designer file doesn't need to be edited
            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnRemove.Click += BtnRemove_Click;
            btnRefresh.Click += BtnRefresh_Click;
            btnSearch.Click += BtnSearch_Click;
            btnShowAll.Click += BtnShowAll_Click;

            // Wire student id leave handler to detect existing student
            txtStudentId.Leave += TxtStudentId_Leave;

            // Initial button states
            btnAdd.Enabled = true;
            btnEdit.Enabled = false;
            btnRemove.Enabled = false;

            dgvStudents.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvStudents.MultiSelect = false;
            dgvStudents.AutoGenerateColumns = false;

            // configure columns so data appears when AutoGenerateColumns is false
            ConfigureDataGrid();

            // Enter key on search boxes triggers search
            txtSearch.KeyDown += TxtSearch_KeyDown;
            txtSearchValue.KeyDown += TxtSearch_KeyDown;
        }

        private void ConfigureDataGrid()
        {
            if (dgvStudents == null) return;
            dgvStudents.Columns.Clear();

            dgvStudents.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colStudentId",
                HeaderText = "StudentId",
                DataPropertyName = "StudentId",
                ReadOnly = true,
                Width = 90
            });

            dgvStudents.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colFullName",
                HeaderText = "Full Name",
                DataPropertyName = "FullName",
                Width = 160
            });

            dgvStudents.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colEmail",
                HeaderText = "Email",
                DataPropertyName = "Email",
                Width = 180
            });

            dgvStudents.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDate",
                HeaderText = "DOB",
                DataPropertyName = "DateOfBirth",
                Width = 90
            });

            dgvStudents.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPhone",
                HeaderText = "Phone",
                DataPropertyName = "Phone",
                Width = 110
            });

            dgvStudents.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colScore",
                HeaderText = "Score",
                DataPropertyName = "Score",
                Width = 60,
                DefaultCellStyle = { Format = "N1" }
            });

            dgvStudents.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colStatus",
                HeaderText = "Status",
                DataPropertyName = "Status",
                Width = 80
            });

            dgvStudents.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colClass",
                HeaderText = "Class",
                DataPropertyName = "Classes",
                Width = 120
            });

            dgvStudents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeData();
            RefreshGrid(students);

            // Set tab order to traverse top->bottom, left->right
            SetTabOrder();
            // default focus
            txtStudentId.Select();
        }

        private void SetTabOrder()
        {
            // Define an explicit tab order: top-down, left-right roughly by control appearance
            var order = new Control[]
            {
                txtStudentId,
                txtFullName,
                txtEmail,
                dtpDate,
                txtPhone,
                nudScore,
                cbClass,
                cbStatus,
                btnAdd,
                btnEdit,
                btnRemove,
                btnRefresh,
                txtSearch,
                cbSearchField,
                txtSearchValue,
                btnSearch,
                btnShowAll,
                dgvStudents
            };

            for (int i = 0; i < order.Length; i++)
            {
                order[i].TabIndex = i;
            }
        }

        private void InitializeData()
        {
            // Seed some classes that respect the CSExxxx pattern
            var seedCodes = new[] { "CSE1000", "CSE1001", "CSE2002", "CSE3003" };
            foreach (var code in seedCodes)
            {
                if (ClassCodeRegex.IsMatch(code))
                {
                    classes.Add(new ClassModel { Code = code, Name = code + " - Sample" });
                }
            }

            // Populate UI pickers
            cbClass.Items.Clear();
            cbClass.Items.AddRange(classes.Select(c => c.Code).ToArray());
            if (cbClass.Items.Count > 0) cbClass.SelectedIndex = 0;

            cbStatus.Items.Clear();
            cbStatus.Items.AddRange(new[] { "Active", "Inactive" });
            cbStatus.SelectedIndex = 0;

            cbSearchField.Items.Clear();
            cbSearchField.Items.AddRange(new[] { "StudentId", "FullName", "Email", "Class" });
            cbSearchField.SelectedIndex = 0;

            // Seed sample students for testing
            var c1 = classes.FirstOrDefault(c => c.Code == "CSE1000");
            var c2 = classes.FirstOrDefault(c => c.Code == "CSE1001");
            var c3 = classes.FirstOrDefault(c => c.Code == "CSE2002");

            var sampleStudents = new[]
            {
                new Student { StudentId = "SV1000", FullName = "Nguyen Van A", Email = "a.van@example.com", DateOfBirth = new DateTime(2000,1,15), Phone = "0901234567", Score = 8.5, Status = "Active", Class = c1 },
                new Student { StudentId = "SV1001", FullName = "Tran Thi B", Email = "b.tran@example.com", DateOfBirth = new DateTime(1999,5,30), Phone = "0912345678", Score = 7.2, Status = "Active", Class = c2 },
                new Student { StudentId = "SV1002", FullName = "Le Van C", Email = "c.le@example.com", DateOfBirth = new DateTime(2001,3,20), Phone = "0987654321", Score = 9.1, Status = "Inactive", Class = c1 },
                new Student { StudentId = "SV1003", FullName = "Pham Thi D", Email = "d.pham@example.com", DateOfBirth = new DateTime(2002,11,5), Phone = "0977123456", Score = 6.0, Status = "Active", Class = c3 }
            };

            foreach (var s in sampleStudents)
            {
                // ensure class list link
                if (s.Class != null)
                {
                    s.Class.Students.Add(s);
                }
                students.Add(s);
            }
        }

        private void RefreshGrid(IEnumerable<Student> source)
        {
            var view = source.Select(s => new StudentView
            {
                StudentId = s.StudentId,
                FullName = s.FullName,
                Email = s.Email,
                DateOfBirth = s.DateOfBirth.ToShortDateString(),
                Phone = s.Phone,
                Score = s.Score,
                Status = s.Status,
                Classes = s.Class != null ? s.Class.Code : string.Empty
            }).ToList();

            dgvStudents.DataSource = new BindingSource { DataSource = view };
            txtTotal.Text = view.Count.ToString();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var id = txtStudentId.Text?.Trim();
            if (string.IsNullOrEmpty(id))
            {
                MessageBox.Show("StudentId is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (students.Any(s => s.StudentId.Equals(id, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("StudentId already exists.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedClassCode = cbClass.SelectedItem as string;
            ClassModel selectedClass = null;
            if (!string.IsNullOrEmpty(selectedClassCode))
            {
                if (!ClassCodeRegex.IsMatch(selectedClassCode))
                {
                    MessageBox.Show("Class code must follow CSE + 4 digits.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                selectedClass = classes.FirstOrDefault(c => c.Code.Equals(selectedClassCode, StringComparison.OrdinalIgnoreCase));
                if (selectedClass == null)
                {
                    selectedClass = new ClassModel { Code = selectedClassCode, Name = selectedClassCode };
                    classes.Add(selectedClass);
                    cbClass.Items.Add(selectedClass.Code);
                }
            }

            var student = new Student
            {
                StudentId = id,
                FullName = txtFullName.Text?.Trim() ?? string.Empty,
                Email = txtEmail.Text?.Trim() ?? string.Empty,
                DateOfBirth = dtpDate.Value,
                Phone = txtPhone.Text?.Trim() ?? string.Empty,
                Score = (double)nudScore.Value,
                Status = cbStatus.SelectedItem as string ?? string.Empty,
                Class = selectedClass
            };

            if (selectedClass != null)
            {
                selectedClass.Students.Add(student);
            }

            // validate
            if (!ValidateModel(student, out var errors))
            {
                MessageBox.Show(string.Join("\n", errors.Select(r => r.ErrorMessage)), "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            students.Add(student);
            RefreshGrid(students);
            ClearInputs();
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            // Confirm before performing an edit (dangerous/overwrite action)
            if (dgvStudents.CurrentRow != null)
            {
                var idConfirm = dgvStudents.CurrentRow.Cells[0].Value as string ?? "";
                var ok = MessageBox.Show($"Bạn có chắc chắn muốn cập nhật thông tin sinh viên '{idConfirm}'?\nHành động này sẽ ghi đè dữ liệu hiện tại.", "Xác nhận cập nhật", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (ok != DialogResult.Yes) return;
            }
            if (dgvStudents.CurrentRow == null) return;
            var id = dgvStudents.CurrentRow.Cells[0].Value as string; // StudentId column
            var student = students.FirstOrDefault(s => s.StudentId.Equals(id, StringComparison.OrdinalIgnoreCase));
            if (student == null) return;

            // Update fields from UI
            student.FullName = txtFullName.Text?.Trim() ?? string.Empty;
            student.Email = txtEmail.Text?.Trim() ?? string.Empty;
            student.DateOfBirth = dtpDate.Value;
            student.Phone = txtPhone.Text?.Trim() ?? string.Empty;
            student.Score = (double)nudScore.Value;
            student.Status = cbStatus.SelectedItem as string ?? string.Empty;

            var selectedClassCode = cbClass.SelectedItem as string;
            if (!string.IsNullOrEmpty(selectedClassCode) && ClassCodeRegex.IsMatch(selectedClassCode))
            {
                var cls = classes.FirstOrDefault(c => c.Code.Equals(selectedClassCode, StringComparison.OrdinalIgnoreCase));
                if (cls == null)
                {
                    cls = new ClassModel { Code = selectedClassCode, Name = selectedClassCode };
                    classes.Add(cls);
                    cbClass.Items.Add(cls.Code);
                }

                // Move student to new class if different
                if (student.Class == null || !student.Class.Code.Equals(cls.Code, StringComparison.OrdinalIgnoreCase))
                {
                    if (student.Class != null)
                    {
                        student.Class.Students.Remove(student);
                    }
                    student.Class = cls;
                    cls.Students.Add(student);
                }
            }

            // validate
            if (!ValidateModel(student, out var errors))
            {
                MessageBox.Show(string.Join("\n", errors.Select(r => r.ErrorMessage)), "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            RefreshGrid(students);
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            if (dgvStudents.CurrentRow == null) return;
            var id = dgvStudents.CurrentRow.Cells[0].Value as string;
            var student = students.FirstOrDefault(s => s.StudentId.Equals(id, StringComparison.OrdinalIgnoreCase));
            if (student == null) return;

            var confirm = MessageBox.Show($"Bạn có thực sự muốn xóa sinh viên '{id}'? Hành động này không thể hoàn tác.", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;

            // Remove from class
            if (student.Class != null)
            {
                student.Class.Students.Remove(student);
            }

            students.Remove(student);
            RefreshGrid(students);
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show("Bạn có muốn làm mới dữ liệu? Tất cả thay đổi chưa lưu (nếu có) sẽ bị ghi đè.", "Xác nhận làm mới", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;
            RefreshGrid(students);
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            var field = cbSearchField.SelectedItem as string ?? "StudentId";
            var keyword = txtSearchValue.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(keyword))
            {
                RefreshGrid(students);
                return;
            }

            IEnumerable<Student> result = students;
            keyword = keyword.ToLowerInvariant();

            switch (field)
            {
                case "StudentId":
                    result = students.Where(s => s.StudentId != null && s.StudentId.ToLowerInvariant().Contains(keyword));
                    break;
                case "FullName":
                    result = students.Where(s => s.FullName != null && s.FullName.ToLowerInvariant().Contains(keyword));
                    break;
                case "Email":
                    result = students.Where(s => s.Email != null && s.Email.ToLowerInvariant().Contains(keyword));
                    break;
                case "Class":
                    result = students.Where(s => s.Class != null && s.Class.Code != null && s.Class.Code.ToLowerInvariant().Contains(keyword));
                    break;
                default:
                    result = students;
                    break;
            }

            RefreshGrid(result);
        }

        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                BtnSearch_Click(sender, EventArgs.Empty);
            }
        }

        private void BtnShowAll_Click(object sender, EventArgs e)
        {
            RefreshGrid(students);
        }

        private static bool ValidateModel(object model, out IList<ValidationResult> results)
        {
            var ctx = new ValidationContext(model, serviceProvider: null, items: null);
            results = new List<ValidationResult>();
            return Validator.TryValidateObject(model, ctx, results, validateAllProperties: true);
        }

        private void ClearInputs()
        {
            txtStudentId.Clear();
            txtFullName.Clear();
            txtEmail.Clear();
            txtPhone.Clear();
            nudScore.Value = 0;
            if (cbClass.Items.Count > 0) cbClass.SelectedIndex = 0;
            if (cbStatus.Items.Count > 0) cbStatus.SelectedIndex = 0;
        }

        // Legacy designer event handlers (kept to avoid designer edit) - forward to new implementations where appropriate
        private void groupBox1_Enter(object sender, EventArgs e) { }
        private void radioButton5_CheckedChanged(object sender, EventArgs e) { }
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void MSV_TextChanged(object sender, EventArgs e) { }
        private void button2_Click(object sender, EventArgs e) { BtnAdd_Click(sender, e); }
        private void button3_Click(object sender, EventArgs e) { BtnRemove_Click(sender, e); }
        private void button6_Click(object sender, EventArgs e) { BtnShowAll_Click(sender, e); }

        private void TxtStudentId_Leave(object sender, EventArgs e)
        {
            var id = txtStudentId.Text?.Trim();
            if (string.IsNullOrEmpty(id))
            {
                // no id entered => prepare for add
                btnAdd.Enabled = true;
                btnEdit.Enabled = false;
                btnRemove.Enabled = false;
                ClearInputs();
                txtStudentId.Text = string.Empty;
                return;
            }

            var existing = students.FirstOrDefault(s => s.StudentId.Equals(id, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                // populate controls and enable edit/delete
                txtFullName.Text = existing.FullName;
                txtEmail.Text = existing.Email;
                dtpDate.Value = existing.DateOfBirth == default ? DateTime.Today : existing.DateOfBirth;
                txtPhone.Text = existing.Phone;
                nudScore.Value = (decimal)existing.Score;
                cbStatus.SelectedItem = existing.Status;
                if (existing.Class != null)
                {
                    var idx = cbClass.Items.IndexOf(existing.Class.Code);
                    if (idx >= 0) cbClass.SelectedIndex = idx;
                }

                btnAdd.Enabled = false;
                btnEdit.Enabled = true;
                btnRemove.Enabled = true;
            }
            else
            {
                // new student: clear other fields and enable add
                txtFullName.Clear();
                txtEmail.Clear();
                dtpDate.Value = DateTime.Today;
                txtPhone.Clear();
                nudScore.Value = 0;
                if (cbClass.Items.Count > 0) cbClass.SelectedIndex = 0;

                btnAdd.Enabled = true;
                btnEdit.Enabled = false;
                btnRemove.Enabled = false;
            }
        }

        // Simple view used for DataGridView binding
        private class StudentView
        {
            public string StudentId { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
            public string DateOfBirth { get; set; }
            public string Phone { get; set; }
            public double Score { get; set; }
            public string Status { get; set; }
            public string Classes { get; set; }
        }

        // Domain models (kept small and in-memory for this example)
        private class Student
        {
            [Required]
            [RegularExpression("^[A-Za-z0-9]{1,20}$")]
            public string StudentId { get; set; }

            [Required]
            [StringLength(100, MinimumLength = 2)]
            public string FullName { get; set; }

            [EmailAddress]
            public string Email { get; set; }

            [Required]
            public DateTime DateOfBirth { get; set; }

            [Phone]
            public string Phone { get; set; }

            [Range(0, 10)]
            public double Score { get; set; }

            public string Status { get; set; }

            // One-to-many: one class per student
            public ClassModel Class { get; set; }
        }

        private class ClassModel
        {
            [Required]
            [RegularExpression("^CSE\\d{4}$", ErrorMessage = "Code must be in format CSENNNN")]
            public string Code { get; set; }

            [Required]
            public string Name { get; set; }

            public List<Student> Students { get; set; } = new();
        }
    }
}
