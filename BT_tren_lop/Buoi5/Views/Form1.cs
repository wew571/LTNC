using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ProjectQLsinhvien.BUL;
using ProjectQLsinhvien.Entity;

namespace ProjectQLsinhvien.Views
{
    /// <summary>
    /// TẦNG VIEWS (Giao diện) – chỉ:
    ///   • Bắt sự kiện chuột/bàn phím,
    ///   • Hiển thị dữ liệu lên điều khiển,
    ///   • Đọc dữ liệu từ form thành Entity và GỌI tầng BUL.
    /// KHÔNG truy cập DAL trực tiếp, KHÔNG tự viết rule nghiệp vụ.
    /// </summary>
    public partial class Form1 : Form
    {
        // =====================================================================
        //  KẾT NỐI CÁC TẦNG (Views → BUL → DAL)
        // =====================================================================

        private readonly LopBus lopBus = new LopBus();
        private readonly SinhVienBUL sinhVienBUL = new SinhVienBUL();

        // =====================================================================
        //  BIẾN CẤP LỚP
        // =====================================================================

        /// <summary>Bảng dữ liệu trung gian để hiển thị lên dataGridView1.</summary>
        private readonly DataTable dtBang = new DataTable("SinhVien");

        private const string Placeholder = "Mã, Họ và tên, Email,...";
        private bool daKhoiTao = false;

        /// <summary>Mã SV đã xử lý lần gần nhất – tránh đổ lại dữ liệu khi mã không đổi.</summary>
        private string maDaXuLy = string.Empty;

        // =====================================================================
        //  CONSTRUCTOR – nối các sự kiện CHƯA có trong Form1.Designer.cs
        // =====================================================================

        public Form1()
        {
            InitializeComponent();

            button2.Click += button2_Click;     // Sửa   (Task 6)
            button3.Click += button3_Click;     // Xóa   (Task 6)
            button4.Click += button4_Click;     // Làm mới (Task 5)
            button5.Click += button5_Click;     // Hiển thị tất cả

            textBox1.Leave += textBox1_Leave;   // Task 4 – rời ô Mã sinh viên

            textBox5.Enter += textBox5_Enter;
            textBox5.Leave += textBox5_Leave;

            dataGridView1.CellClick += dataGridView1_CellClick;
        }

        // =====================================================================
        //  FORM LOAD
        // =====================================================================

        private void Form1_Load(object sender, EventArgs e)
        {
            TaoCauTrucBang();                    // cấu trúc + liên kết lưới
            TaoDanhSachLop();                    // ★ gọi BUL nạp lớp vào comboBox1/3

            dateTimePicker1.MaxDate = DateTime.Today;
            dateTimePicker1.Value = DateTime.Today;

            numericUpDown1.Minimum = 0;
            numericUpDown1.Maximum = 10;
            numericUpDown1.DecimalPlaces = 1;
            numericUpDown1.Increment = 0.1M;
            numericUpDown1.Value = 0;

            numericUpDown2.Minimum = 0;
            numericUpDown2.Maximum = 10;
            numericUpDown2.DecimalPlaces = 1;
            numericUpDown2.Increment = 0.1M;
            numericUpDown2.Value = 0;

            comboBox2.SelectedIndex = 0;                 // Trạng thái: "Đang học"
            textBox5.ForeColor = SystemColors.ScrollBar;

            NapBangLenGrid();                    // ★ gọi BUL nạp danh sách sinh viên
            DatThuTuTabIndex();
            DatTrangThaiNut(maTonTai: false);     // Bật Thêm, tắt Sửa/Xóa

            daKhoiTao = true;
            ActiveControl = textBox1;            // Focus tại ô Mã sinh viên
        }

        private void TaoCauTrucBang()
        {
            dtBang.Columns.Add("MaSV", typeof(string));
            dtBang.Columns.Add("HoTen", typeof(string));
            dtBang.Columns.Add("NgaySinh", typeof(DateTime));
            dtBang.Columns.Add("GioiTinh", typeof(string));
            dtBang.Columns.Add("DienThoai", typeof(string));
            dtBang.Columns.Add("Email", typeof(string));
            dtBang.Columns.Add("LopHoc", typeof(string));
            dtBang.Columns.Add("Diem", typeof(double));
            dtBang.Columns.Add("TrangThai", typeof(string));

            dataGridView1.DataSource = dtBang;

            string[] tieuDe =
            {
                "Mã SV", "Họ và tên", "Ngày sinh", "Giới tính", "Điện thoại",
                "Email", "Lớp học", "Điểm", "Trạng thái"
            };
            for (int i = 0; i < tieuDe.Length && i < dataGridView1.Columns.Count; i++)
                dataGridView1.Columns[i].HeaderText = tieuDe[i];

            if (dataGridView1.Columns["NgaySinh"] != null)
                dataGridView1.Columns["NgaySinh"].DefaultCellStyle.Format = "dd/MM/yyyy";

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        /// <summary>★ Nạp lớp từ tầng BUL vào comboBox1 (nhập) và comboBox3 (tìm kiếm).</summary>
        private void TaoDanhSachLop()
        {
            List<LopHoc> dsLop = lopBus.GetAllLopHoc();

            comboBox1.DisplayMember = "TenLop";
            comboBox1.ValueMember = "MaLop";
            comboBox1.DataSource = new List<LopHoc>(dsLop);   // bản sao → 2 combo không dính nhau
            comboBox1.SelectedIndex = -1;

            comboBox3.DisplayMember = "TenLop";
            comboBox3.ValueMember = "MaLop";
            comboBox3.DataSource = new List<LopHoc>(dsLop);
            comboBox3.SelectedIndex = -1;
        }

        private void DatThuTuTabIndex()
        {
            textBox1.TabIndex = 0;
            textBox4.TabIndex = 1;
            dateTimePicker1.TabIndex = 2;
            radioButton1.TabIndex = 3;
            radioButton2.TabIndex = 4;
            textBox2.TabIndex = 5;
            textBox3.TabIndex = 6;
            comboBox1.TabIndex = 7;
            numericUpDown1.TabIndex = 8;
            comboBox2.TabIndex = 9;
            button1.TabIndex = 10;
            button2.TabIndex = 11;
            button3.TabIndex = 12;
            button4.TabIndex = 13;
            textBox5.TabIndex = 14;
            comboBox3.TabIndex = 15;
            numericUpDown2.TabIndex = 16;
            button7.TabIndex = 17;
            button5.TabIndex = 18;
            dataGridView1.TabIndex = 19;
        }

        // =====================================================================
        //  TASK 4 – NHẬP / RỜI Ô MÃ SINH VIÊN
        // =====================================================================

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {
            if (!daKhoiTao) return;
            XuLyMaSinhVien();
        }

        private void textBox1_Leave(object? sender, EventArgs e)
        {
            if (!daKhoiTao) return;
            XuLyMaSinhVien();
        }

        /// <summary>
        /// • Mã ĐÃ TỒN TẠI  → đổ dữ liệu lên form, disable Thêm, enable Sửa/Xóa.
        /// • Mã CHƯA TỒN TẠI → xóa trắng các ô khác, enable Thêm, disable Sửa/Xóa.
        /// </summary>
        private void XuLyMaSinhVien()
        {
            string maSV = textBox1.Text.Trim();

            if (string.Equals(maSV, maDaXuLy, StringComparison.OrdinalIgnoreCase))
                return;                       // mã chưa đổi → không đụng dữ liệu đang nhập

            maDaXuLy = maSV;

            SinhVien? sv = sinhVienBUL.GetSinhVienById(maSV);   // ★ gọi BUL

            if (sv != null)
            {
                DoDuLieuLenForm(sv, capNhatMaSV: false);
                DatTrangThaiNut(maTonTai: true);
            }
            else
            {
                XoaTrangCacONhap();
                DatTrangThaiNut(maTonTai: false);
            }
        }

        private void DatTrangThaiNut(bool maTonTai)
        {
            button1.Enabled = !maTonTai;   // Thêm
            button2.Enabled = maTonTai;    // Sửa
            button3.Enabled = maTonTai;    // Xóa
        }

        private void DoDuLieuLenForm(SinhVien sv, bool capNhatMaSV = true)
        {
            if (capNhatMaSV)
                textBox1.Text = sv.MaSV;

            textBox4.Text = sv.HoTen;

            if (sv.NgaySinh <= dateTimePicker1.MaxDate)
                dateTimePicker1.Value = sv.NgaySinh;

            radioButton1.Checked = (sv.GioiTinh == "Nam");
            radioButton2.Checked = (sv.GioiTinh == "Nữ");

            textBox2.Text = sv.DienThoai ?? string.Empty;
            textBox3.Text = sv.Email ?? string.Empty;

            ChonLop(comboBox1, sv.MaLop);

            numericUpDown1.Value = (decimal)Math.Clamp(sv.Diem,
                (double)numericUpDown1.Minimum, (double)numericUpDown1.Maximum);

            comboBox2.SelectedIndex = comboBox2.FindStringExact(sv.TrangThai);
        }

        private void XoaTrangCacONhap()
        {
            textBox4.Clear();
            textBox2.Clear();
            textBox3.Clear();
            dateTimePicker1.Value = DateTime.Today;
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            numericUpDown1.Value = 0;
            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = 0;
        }

        // =====================================================================
        //  TASK 5 – NÚT LÀM MỚI (button4)
        // =====================================================================

        private void button4_Click(object? sender, EventArgs e)
        {
            LamMoiNhapLieu();
        }

        private void LamMoiNhapLieu()
        {
            textBox1.Clear();
            XoaTrangCacONhap();
            maDaXuLy = string.Empty;

            textBox5.Text = Placeholder;
            textBox5.ForeColor = SystemColors.ScrollBar;
            comboBox3.SelectedIndex = -1;
            numericUpDown2.Value = 0;

            NapBangLenGrid();                 // trả lưới về toàn bộ danh sách

            DatTrangThaiNut(maTonTai: false);
            textBox1.Focus();                 // ★ trả con trỏ về ô Mã sinh viên
        }

        // =====================================================================
        //  TASK 6 – THÊM / SỬA / XÓA (Xóa & Sửa bắt buộc hiện YesNo)
        // =====================================================================

        /// <summary>NÚT THÊM (button1).</summary>
        private void button1_Click(object sender, EventArgs e)
        {
            SinhVien sv = DocDuLieuTuForm();

            try
            {
                sinhVienBUL.ThemSinhVien(sv);          // ★ Views → BUL → DAL
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DatTrangThaiNut(sinhVienBUL.KiemTraTonTai(sv.MaSV));
                return;
            }

            NapBangLenGrid();
            MessageBox.Show("Thêm sinh viên thành công!", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            LamMoiNhapLieu();
        }

        /// <summary>NÚT SỬA (button2) – bắt buộc xác nhận YesNo.</summary>
        private void button2_Click(object? sender, EventArgs e)
        {
            SinhVien svMoi = DocDuLieuTuForm();

            // Validate trước, để khỏi hiện hộp thoại xác nhận cho dữ liệu lỗi
            if (!svMoi.KiemTraHopLe(out string loi))
            {
                MessageBox.Show(loi, "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SinhVien? svCu = sinhVienBUL.GetSinhVienById(svMoi.MaSV);
            if (svCu == null)
            {
                MessageBox.Show("Không tìm thấy sinh viên có mã \"" + svMoi.MaSV + "\"!",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DatTrangThaiNut(false);
                return;
            }

            // ★ Task 6: hộp thoại xác nhận TRƯỚC KHI thực hiện
            DialogResult kq = MessageBox.Show(
                "Bạn có chắc chắn muốn CẬP NHẬT thông tin của sinh viên\n" +
                "\"" + svCu.MaSV + " - " + svCu.HoTen + "\" không?",
                "Xác nhận sửa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (kq != DialogResult.Yes) return;         // chọn No → hủy

            try
            {
                sinhVienBUL.CapNhatSinhVien(svMoi);     // ★ Views → BUL → DAL
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            NapBangLenGrid();
            MessageBox.Show("Cập nhật thông tin sinh viên thành công!", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            LamMoiNhapLieu();
        }

        /// <summary>NÚT XÓA (button3) – bắt buộc xác nhận YesNo.</summary>
        private void button3_Click(object? sender, EventArgs e)
        {
            string maSV = textBox1.Text.Trim();
            SinhVien? sv = sinhVienBUL.GetSinhVienById(maSV);

            if (sv == null)
            {
                MessageBox.Show("Không tìm thấy sinh viên cần xóa!",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DatTrangThaiNut(false);
                return;
            }

            // ★ Task 6: hộp thoại xác nhận TRƯỚC KHI thực hiện
            DialogResult kq = MessageBox.Show(
                "Bạn có chắc chắn muốn XÓA sinh viên\n" +
                "\"" + sv.MaSV + " - " + sv.HoTen + "\" không?\n" +
                "Hành động này không thể hoàn tác.",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (kq != DialogResult.Yes) return;         // chọn No → hủy

            try
            {
                sinhVienBUL.XoaSinhVien(maSV);          // ★ Views → BUL → DAL
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            NapBangLenGrid();
            MessageBox.Show("Đã xóa sinh viên \"" + sv.MaSV + "\"!", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            LamMoiNhapLieu();
        }

        /// <summary>NÚT HIỂN THỊ TẤT CẢ (button5).</summary>
        private void button5_Click(object? sender, EventArgs e)
        {
            textBox5.Text = Placeholder;
            textBox5.ForeColor = SystemColors.ScrollBar;
            comboBox3.SelectedIndex = -1;
            numericUpDown2.Value = 0;
            NapBangLenGrid();
        }

        // =====================================================================
        //  TÌM KIẾM
        // =====================================================================

        private void button7_Click(object sender, EventArgs e)
        {
            if (!daKhoiTao) return;

            string tuKhoa = LayTuKhoa();
            string? maLop = (comboBox3.SelectedItem as LopHoc)?.MaLop;
            double diemToiThieu = (double)numericUpDown2.Value;

            // ★ Logic lọc nằm ở BUL – Form chỉ truyền tiêu chí và nhận kết quả
            List<SinhVien> dsLoc = sinhVienBUL.TimKiem(tuKhoa, maLop, diemToiThieu);

            NapBangLenGrid(dsLoc);

            if (dsLoc.Count == 0)
                MessageBox.Show("Không tìm thấy sinh viên phù hợp!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private string LayTuKhoa()
        {
            string tuKhoa = textBox5.Text.Trim();
            return string.Equals(tuKhoa, Placeholder, StringComparison.OrdinalIgnoreCase)
                ? string.Empty
                : tuKhoa;
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (!daKhoiTao) return;

            bool laChuMo = string.Equals(textBox5.Text.Trim(), Placeholder,
                StringComparison.OrdinalIgnoreCase);
            textBox5.ForeColor = laChuMo ? SystemColors.ScrollBar : SystemColors.WindowText;
        }

        private void textBox5_Enter(object? sender, EventArgs e)
        {
            if (string.Equals(textBox5.Text.Trim(), Placeholder,
                    StringComparison.OrdinalIgnoreCase))
            {
                textBox5.Text = string.Empty;
                textBox5.ForeColor = SystemColors.WindowText;
            }
        }

        private void textBox5_Leave(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox5.Text))
            {
                textBox5.Text = Placeholder;
                textBox5.ForeColor = SystemColors.ScrollBar;
            }
        }

        // =====================================================================
        //  CHỌN DÒNG TRÊ dataGridView1
        // =====================================================================

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            ChonSinhVienTuGrid(e.RowIndex);
        }

        private void dataGridView1_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            ChonSinhVienTuGrid(e.RowIndex);
        }

        private void ChonSinhVienTuGrid(int rowIndex)
        {
            if (!daKhoiTao || rowIndex < 0 || rowIndex >= dtBang.Rows.Count)
                return;

            string maSV = dtBang.Rows[rowIndex]["MaSV"].ToString() ?? string.Empty;
            SinhVien? sv = sinhVienBUL.GetSinhVienById(maSV);
            if (sv == null) return;

            DoDuLieuLenForm(sv);
            maDaXuLy = sv.MaSV;
            DatTrangThaiNut(maTonTai: true);
        }

        // =====================================================================
        //  HÀM HỖ TRỢ (View-side)
        // =====================================================================

        /// <summary>Đọc điều khiển trên Form và tạo 1 Entity SinhVien.</summary>
        private SinhVien DocDuLieuTuForm()
        {
            LopHoc? lop = comboBox1.SelectedItem as LopHoc;

            return new SinhVien
            {
                MaSV = textBox1.Text.Trim(),
                HoTen = textBox4.Text.Trim(),
                NgaySinh = dateTimePicker1.Value.Date,
                GioiTinh = radioButton1.Checked ? "Nam" : (radioButton2.Checked ? "Nữ" : string.Empty),
                DienThoai = string.IsNullOrWhiteSpace(textBox2.Text) ? null : textBox2.Text.Trim(),
                Email = string.IsNullOrWhiteSpace(textBox3.Text) ? null : textBox3.Text.Trim(),
                MaLop = lop?.MaLop ?? string.Empty,
                Lop = lop,
                Diem = (double)numericUpDown1.Value,
                TrangThai = comboBox2.SelectedItem?.ToString() ?? string.Empty
            };
        }

        /// <summary>Chọn lớp trong ComboBox theo mã lớp.</summary>
        private void ChonLop(ComboBox combo, string maLop)
        {
            for (int i = 0; i < combo.Items.Count; i++)
            {
                if (combo.Items[i] is LopHoc lop && lop.MaLop == maLop)
                {
                    combo.SelectedIndex = i;
                    return;
                }
            }
            combo.SelectedIndex = -1;
        }

        /// <summary>Lấy tên lớp hiển thị trên lưới.</summary>
        private string TenLopCua(string maLop) => lopBus.GetLopById(maLop)?.TenLop ?? maLop;

        /// <summary>Nạp dữ liệu (mặc định lấy toàn bộ từ BUL) lên dataGridView1.</summary>
        private void NapBangLenGrid(IEnumerable<SinhVien>? dsHienThi = null)
        {
            dtBang.Rows.Clear();

            foreach (SinhVien sv in (dsHienThi ?? sinhVienBUL.GetAllSinhVien()))
            {
                dtBang.Rows.Add(
                    sv.MaSV, sv.HoTen, sv.NgaySinh, sv.GioiTinh,
                    sv.DienThoai ?? string.Empty, sv.Email ?? string.Empty,
                    TenLopCua(sv.MaLop), sv.Diem, sv.TrangThai);
            }

            dataGridView1.ClearSelection();
        }

        // =====================================================================
        //  CÁC SỰ KIỆN KHÁC NỐI TRONG DESIGNER (giữ để không lỗi biên dịch)
        // =====================================================================

        private void label1_Click(object sender, EventArgs e) { }

        private void label13_Click(object sender, EventArgs e) { }

        private void label14_Click_1(object sender, EventArgs e) { }

        private void label17_Click(object sender, EventArgs e) { }

        private void label20_Click(object sender, EventArgs e) { }

        private void groupBox1_Enter(object sender, EventArgs e) { }

        private void groupBox2_Enter(object sender, EventArgs e) { }

        private void groupBox3_Enter(object sender, EventArgs e) { }

        private void textBox2_TextChanged(object sender, EventArgs e) { }

        private void textBox4_TextChanged(object sender, EventArgs e) { }

        private void radioButton1_CheckedChanged(object sender, EventArgs e) { }
    }
}
