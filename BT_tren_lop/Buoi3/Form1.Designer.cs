namespace WinFormsApp2
{
    partial class QLSV
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            gbStudentInfo = new GroupBox();
            cbClass = new ComboBox();
            cbStatus = new ComboBox();
            nudScore = new NumericUpDown();
            lblStatus = new Label();
            lblScore = new Label();
            lblClass = new Label();
            rbMale = new RadioButton();
            rbFemale = new RadioButton();
            lblSex = new Label();
            txtPhone = new TextBox();
            lblPhone = new Label();
            lblFullName = new Label();
            txtFullName = new TextBox();
            dtpDate = new DateTimePicker();
            lblDate = new Label();
            txtEmail = new TextBox();
            lblEmail = new Label();
            lblStudentId = new Label();
            txtStudentId = new TextBox();
            btnEdit = new Button();
            btnAdd = new Button();
            btnRemove = new Button();
            btnRefresh = new Button();
            gbSearch = new GroupBox();
            btnShowAll = new Button();
            btnSearch = new Button();
            txtSearchValue = new TextBox();
            lblSearchValue = new Label();
            cbSearchField = new ComboBox();
            lblSearchField = new Label();
            txtSearch = new TextBox();
            lblSearchBy = new Label();
            gbResults = new GroupBox();
            txtTotal = new TextBox();
            lblTotal = new Label();
            dgvStudents = new DataGridView();
            gbStudentInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudScore).BeginInit();
            gbSearch.SuspendLayout();
            gbResults.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvStudents).BeginInit();
            SuspendLayout();
            // 
            // gbStudentInfo
            // 
            gbStudentInfo.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gbStudentInfo.Controls.Add(cbClass);
            gbStudentInfo.Controls.Add(cbStatus);
            gbStudentInfo.Controls.Add(nudScore);
            gbStudentInfo.Controls.Add(lblStatus);
            gbStudentInfo.Controls.Add(lblScore);
            gbStudentInfo.Controls.Add(lblClass);
            gbStudentInfo.Controls.Add(rbMale);
            gbStudentInfo.Controls.Add(rbFemale);
            gbStudentInfo.Controls.Add(lblSex);
            gbStudentInfo.Controls.Add(txtPhone);
            gbStudentInfo.Controls.Add(lblPhone);
            gbStudentInfo.Controls.Add(lblFullName);
            gbStudentInfo.Controls.Add(txtFullName);
            gbStudentInfo.Controls.Add(dtpDate);
            gbStudentInfo.Controls.Add(lblDate);
            gbStudentInfo.Controls.Add(txtEmail);
            gbStudentInfo.Controls.Add(lblEmail);
            gbStudentInfo.Controls.Add(lblStudentId);
            gbStudentInfo.Controls.Add(txtStudentId);
            gbStudentInfo.Location = new Point(3, 11);
            gbStudentInfo.Name = "gbStudentInfo";
            gbStudentInfo.Size = new Size(825, 174);
            gbStudentInfo.TabIndex = 0;
            gbStudentInfo.TabStop = false;
            gbStudentInfo.Text = "Thong tin sinh vien";
            gbStudentInfo.Enter += groupBox1_Enter;
            // 
            // cbClass
            // 
            cbClass.FormattingEnabled = true;
            cbClass.Location = new Point(623, 44);
            cbClass.Name = "cbClass";
            cbClass.Size = new Size(151, 28);
            cbClass.TabIndex = 25;
            // 
            // cbStatus
            // 
            cbStatus.FormattingEnabled = true;
            cbStatus.Location = new Point(623, 114);
            cbStatus.Name = "cbStatus";
            cbStatus.Size = new Size(151, 28);
            cbStatus.TabIndex = 24;
            // 
            // nudScore
            // 
            nudScore.Location = new Point(623, 80);
            nudScore.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            nudScore.Name = "nudScore";
            nudScore.Size = new Size(150, 27);
            nudScore.TabIndex = 23;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(566, 114);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(49, 20);
            lblStatus.TabIndex = 22;
            lblStatus.Text = "Status";
            // 
            // lblScore
            // 
            lblScore.AutoSize = true;
            lblScore.Location = new Point(567, 80);
            lblScore.Name = "lblScore";
            lblScore.Size = new Size(46, 20);
            lblScore.TabIndex = 20;
            lblScore.Text = "Score";
            // 
            // lblClass
            // 
            lblClass.AutoSize = true;
            lblClass.Location = new Point(567, 47);
            lblClass.Name = "lblClass";
            lblClass.Size = new Size(42, 20);
            lblClass.TabIndex = 19;
            lblClass.Text = "Class";
            // 
            // rbMale
            // 
            rbMale.AutoSize = true;
            rbMale.Location = new Point(431, 113);
            rbMale.Name = "rbMale";
            rbMale.Size = new Size(43, 24);
            rbMale.TabIndex = 17;
            rbMale.Text = "M";
            rbMale.UseVisualStyleBackColor = true;
            // 
            // rbFemale
            // 
            rbFemale.AutoSize = true;
            rbFemale.Location = new Point(356, 113);
            rbFemale.Name = "rbFemale";
            rbFemale.Size = new Size(50, 24);
            rbFemale.TabIndex = 16;
            rbFemale.Text = "FM";
            rbFemale.UseVisualStyleBackColor = true;
            rbFemale.CheckedChanged += radioButton5_CheckedChanged;
            // 
            // lblSex
            // 
            lblSex.AutoSize = true;
            lblSex.Location = new Point(277, 113);
            lblSex.Name = "lblSex";
            lblSex.Size = new Size(32, 20);
            lblSex.TabIndex = 10;
            lblSex.Text = "Sex";
            // 
            // txtPhone
            // 
            txtPhone.AcceptsReturn = true;
            txtPhone.AccessibleName = "label";
            txtPhone.Location = new Point(356, 74);
            txtPhone.Name = "txtPhone";
            txtPhone.Size = new Size(125, 27);
            txtPhone.TabIndex = 9;
            txtPhone.TextChanged += textBox1_TextChanged;
            // 
            // lblPhone
            // 
            lblPhone.AccessibleName = "label";
            lblPhone.AutoSize = true;
            lblPhone.Location = new Point(277, 77);
            lblPhone.Name = "lblPhone";
            lblPhone.Size = new Size(50, 20);
            lblPhone.TabIndex = 8;
            lblPhone.Text = "Phone";
            // 
            // lblFullName
            // 
            lblFullName.AccessibleName = "label";
            lblFullName.AutoSize = true;
            lblFullName.Location = new Point(277, 44);
            lblFullName.Name = "lblFullName";
            lblFullName.Size = new Size(73, 20);
            lblFullName.TabIndex = 7;
            lblFullName.Text = "Full name";
            // 
            // txtFullName
            // 
            txtFullName.AccessibleName = "label";
            txtFullName.Location = new Point(356, 41);
            txtFullName.Name = "txtFullName";
            txtFullName.Size = new Size(125, 27);
            txtFullName.TabIndex = 6;
            txtFullName.TextChanged += textBox2_TextChanged;
            // 
            // dtpDate
            // 
            dtpDate.CustomFormat = "dd/MM/yyyy";
            dtpDate.Format = DateTimePickerFormat.Custom;
            dtpDate.Location = new Point(73, 108);
            dtpDate.Name = "dtpDate";
            dtpDate.Size = new Size(106, 27);
            dtpDate.TabIndex = 5;
            dtpDate.Value = new DateTime(2026, 10, 15, 0, 0, 0, 0);
            // 
            // lblDate
            // 
            lblDate.AutoSize = true;
            lblDate.Location = new Point(16, 108);
            lblDate.Name = "lblDate";
            lblDate.Size = new Size(41, 20);
            lblDate.TabIndex = 4;
            lblDate.Text = "Date";
            // 
            // txtEmail
            // 
            txtEmail.AcceptsReturn = true;
            txtEmail.Location = new Point(73, 71);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(125, 27);
            txtEmail.TabIndex = 3;
            // 
            // lblEmail
            // 
            lblEmail.AutoSize = true;
            lblEmail.Location = new Point(17, 74);
            lblEmail.Name = "lblEmail";
            lblEmail.Size = new Size(46, 20);
            lblEmail.TabIndex = 2;
            lblEmail.Text = "Email";
            // 
            // lblStudentId
            // 
            lblStudentId.AutoSize = true;
            lblStudentId.Location = new Point(17, 41);
            lblStudentId.Name = "lblStudentId";
            lblStudentId.Size = new Size(30, 20);
            lblStudentId.TabIndex = 1;
            lblStudentId.Text = "ID*";
            // 
            // txtStudentId
            // 
            txtStudentId.Location = new Point(73, 38);
            txtStudentId.Name = "txtStudentId";
            txtStudentId.Size = new Size(125, 27);
            txtStudentId.TabIndex = 0;
            txtStudentId.TextChanged += MSV_TextChanged;
            // 
            // btnEdit
            // 
            btnEdit.Location = new Point(490, 205);
            btnEdit.Name = "btnEdit";
            btnEdit.Size = new Size(94, 29);
            btnEdit.TabIndex = 1;
            btnEdit.Text = "Edit";
            btnEdit.UseVisualStyleBackColor = true;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(390, 205);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(94, 29);
            btnAdd.TabIndex = 2;
            btnAdd.Text = "Add";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += button2_Click;
            // 
            // btnRemove
            // 
            btnRemove.Location = new Point(590, 205);
            btnRemove.Name = "btnRemove";
            btnRemove.Size = new Size(94, 29);
            btnRemove.TabIndex = 3;
            btnRemove.Text = "Remove";
            btnRemove.UseVisualStyleBackColor = true;
            btnRemove.Click += button3_Click;
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(690, 205);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(94, 29);
            btnRefresh.TabIndex = 4;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            // 
            // gbSearch
            // 
            gbSearch.Controls.Add(btnShowAll);
            gbSearch.Controls.Add(btnSearch);
            gbSearch.Controls.Add(txtSearchValue);
            gbSearch.Controls.Add(lblSearchValue);
            gbSearch.Controls.Add(cbSearchField);
            gbSearch.Controls.Add(lblSearchField);
            gbSearch.Controls.Add(txtSearch);
            gbSearch.Controls.Add(lblSearchBy);
            gbSearch.Location = new Point(9, 257);
            gbSearch.Name = "gbSearch";
            gbSearch.Size = new Size(819, 62);
            gbSearch.TabIndex = 5;
            gbSearch.TabStop = false;
            gbSearch.Text = "Search";
            // 
            // btnShowAll
            // 
            btnShowAll.Location = new Point(709, 19);
            btnShowAll.Name = "btnShowAll";
            btnShowAll.Size = new Size(94, 29);
            btnShowAll.TabIndex = 30;
            btnShowAll.Text = "Show all";
            btnShowAll.UseVisualStyleBackColor = true;
            btnShowAll.Click += button6_Click;
            // 
            // btnSearch
            // 
            btnSearch.Location = new Point(609, 20);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(94, 29);
            btnSearch.TabIndex = 29;
            btnSearch.Text = "Search";
            btnSearch.UseVisualStyleBackColor = true;
            // 
            // txtSearchValue
            // 
            txtSearchValue.AccessibleName = "label";
            txtSearchValue.Location = new Point(516, 20);
            txtSearchValue.Name = "txtSearchValue";
            txtSearchValue.Size = new Size(87, 27);
            txtSearchValue.TabIndex = 28;
            // 
            // lblSearchValue
            // 
            lblSearchValue.AccessibleName = "label";
            lblSearchValue.AutoSize = true;
            lblSearchValue.Location = new Point(465, 23);
            lblSearchValue.Name = "lblSearchValue";
            lblSearchValue.Size = new Size(46, 20);
            lblSearchValue.TabIndex = 27;
            lblSearchValue.Text = "Score";
            // 
            // cbSearchField
            // 
            cbSearchField.FormattingEnabled = true;
            cbSearchField.Location = new Point(381, 20);
            cbSearchField.Name = "cbSearchField";
            cbSearchField.Size = new Size(78, 28);
            cbSearchField.TabIndex = 26;
            // 
            // lblSearchField
            // 
            lblSearchField.AutoSize = true;
            lblSearchField.Location = new Point(338, 23);
            lblSearchField.Name = "lblSearchField";
            lblSearchField.Size = new Size(42, 20);
            lblSearchField.TabIndex = 2;
            lblSearchField.Text = "Class";
            // 
            // txtSearch
            // 
            txtSearch.Location = new Point(88, 20);
            txtSearch.Name = "txtSearch";
            txtSearch.PlaceholderText = "ID,name,phone,..";
            txtSearch.Size = new Size(215, 27);
            txtSearch.TabIndex = 1;
            // 
            // lblSearchBy
            // 
            lblSearchBy.AutoSize = true;
            lblSearchBy.Location = new Point(11, 23);
            lblSearchBy.Name = "lblSearchBy";
            lblSearchBy.Size = new Size(71, 20);
            lblSearchBy.TabIndex = 0;
            lblSearchBy.Text = "Key word";
            // 
            // gbResults
            // 
            gbResults.Controls.Add(txtTotal);
            gbResults.Controls.Add(lblTotal);
            gbResults.Controls.Add(dgvStudents);
            gbResults.Location = new Point(6, 333);
            gbResults.Name = "gbResults";
            gbResults.Size = new Size(822, 282);
            gbResults.TabIndex = 6;
            gbResults.TabStop = false;
            gbResults.Text = "Student info";
            // 
            // txtTotal
            // 
            txtTotal.Location = new Point(759, 16);
            txtTotal.Name = "txtTotal";
            txtTotal.ReadOnly = true;
            txtTotal.Size = new Size(47, 27);
            txtTotal.TabIndex = 3;
            // 
            // lblTotal
            // 
            lblTotal.AutoSize = true;
            lblTotal.Location = new Point(712, 19);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(49, 20);
            lblTotal.TabIndex = 2;
            lblTotal.Text = "Total: ";
            // 
            // dgvStudents
            // 
            dgvStudents.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvStudents.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvStudents.Location = new Point(13, 46);
            dgvStudents.Name = "dgvStudents";
            dgvStudents.RowHeadersWidth = 51;
            dgvStudents.Size = new Size(793, 230);
            dgvStudents.TabIndex = 0;
            // 
            // QLSV
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(840, 650);
            Controls.Add(gbResults);
            Controls.Add(gbSearch);
            Controls.Add(btnRefresh);
            Controls.Add(btnRemove);
            Controls.Add(btnAdd);
            Controls.Add(btnEdit);
            Controls.Add(gbStudentInfo);
            Name = "QLSV";
            Text = "Ung dung quan ly sinh vien";
            Load += Form1_Load;
            gbStudentInfo.ResumeLayout(false);
            gbStudentInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudScore).EndInit();
            gbSearch.ResumeLayout(false);
            gbSearch.PerformLayout();
            gbResults.ResumeLayout(false);
            gbResults.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvStudents).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox gbStudentInfo;
        private Label lblEmail;
        private Label lblStudentId;
        private TextBox txtStudentId;
        private TextBox txtFullName;
        private TextBox txtEmail;
        private Label lblDate;
        private DateTimePicker dtpDate;
        private TextBox txtPhone;
        private Label lblPhone;
        private Label lblFullName;
        private Label lblSex;
        private RadioButton rbFemale;
        private RadioButton rbMale;
        private Label lblStatus;
        private Label lblScore;
        private Label lblClass;
        private NumericUpDown nudScore;
        private ComboBox cbClass;
        private ComboBox cbStatus;
        private Button btnEdit;
        private Button btnAdd;
        private Button btnRemove;
        private Button btnRefresh;
        private GroupBox gbSearch;
        private Label lblSearchBy;
        private TextBox txtSearchValue;
        private Label lblSearchValue;
        private ComboBox cbSearchField;
        private Label lblSearchField;
        private TextBox txtSearch;
        private Button btnSearch;
        private Button btnShowAll;
        private GroupBox gbResults;
        private TextBox txtTotal;
        private Label lblTotal;
        private DataGridView dgvStudents;
    }
}
