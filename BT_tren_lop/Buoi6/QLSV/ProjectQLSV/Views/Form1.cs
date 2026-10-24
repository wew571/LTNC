using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ProjectQLsinhvien.BUL;
using ProjectQLsinhvien.Common;
using ProjectQLsinhvien.Entity;
using ValidationException = ProjectQLsinhvien.Common.ValidationException;

namespace ProjectQLsinhvien.Views
{
    /// <summary>
    /// TẦNG VIEWS (Presentation) – chỉ:
    ///   • Bắt sự kiện chuột/bàn phím,
    ///   • Hiển thị dữ liệu lên điều khiển / ErrorProvider,
    ///   • Đọc dữ liệu từ form thành Entity và GỌI tầng BUL.
    /// KHÔNG truy cập DAL trực tiếp, KHÔNG tự viết rule nghiệp vụ.
    /// XỬ LÝ NGOẠI LỆ: ValidationException → ErrorProvider từng trường;
    /// BusinessException / DataAccessException → MessageBox.
    /// </summary>
    public partial class Form1 : Form
    {
        // =====================================================================
        //  KẾT NỐI CÁC TẦNG (Views → BUL → DAO → MySQL)
        // =====================================================================

        private readonly LopBUL lopBUL = new LopBUL();
        private readonly SinhVienBUL sinhVienBUL = new SinhVienBUL();

        // =====================================================================
        //  BIẾN CẤP LỚP
        // =====================================================================

        /// <summary>Bảng dữ liệu trung gian để hiển thị lên dataGridView1.</summary>
        private readonly DataTable dtBang = new DataTable("SinhVien");

        /// <summary>Hiển thị lỗi TỪNG TRƯỜNG trên form (yêu cầu của đề bài).</summary>
        private readonly ErrorProvider erp = new ErrorProvider();

        /// <summary>Tra cứu nhanh tên lớp theo mã – tránh truy vấn lại khi đổ lưới.</summary>
        private readonly Dictionary<string, string> tenLopTheoMa = new Dictionary<string, string>();

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

            erp.ContainerControl = this;
            erp.BlinkStyle = ErrorBlinkStyle.NeverBlink;

            button2.Click += button2_Click;     // Sửa
            button3.Click += button3_Click;     // Xóa
            button4.Click += button4_Click;     // Làm mới
            button5.Click += button5_Click;     // Hiển thị tất cả

            textBox1.Leave += textBox1_Leave;   // Task 4 – rời ô Mã sinh viên

            textBox5.Enter += textBox5_Enter;
            textBox5.Leave += textBox5_Leave;

            dataGridView1.CellClick += dataGridView1_CellClick;

            // ★ Chọn lớp nào trên comboBox3 → hiển thị sinh viên của lớp đó
            comboBox3.SelectedIndexChanged += comboBox3_SelectedIndexChanged;

            // Bấm vào ô nào thì xóa dấu lỗi ErrorProvider của ô đó
            foreach (Control c in new Control[]
                     { textBox1, textBox4, dateTimePicker1, radioButton1, radioButton2,
                       textBox2, textBox3, comboBox1, numericUpDown1, comboBox2 })
            {
                c.Enter += (_, __) => erp.SetError(c, string.Empty);
            }
        }

        // =====================================================================
        //  FORM LOAD
        // =====================================================================

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                TaoCauTrucBang();                    // cấu trúc + liên kết lưới

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

                TaoDanhSachLop();                    // ★ BUL → DAO → MySQL nạp lớp vào comboBox
                NapBangLenGrid();                    // ★ BUL → DAO → MySQL nạp danh sách sinh viên

                DatThuTuTabIndex();
                DatTrangThaiNut(maTonTai: false);     // Bật Thêm, tắt Sửa/Xóa

                daKhoiTao = true;
                ActiveControl = textBox1;            // Focus tại ô Mã sinh viên
            }
            catch (Exception ex)
            {
                HienThiLoiNgoaiLe(ex);
            }
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

        /// <summary>★ Nạp DANH SÁCH LỚP từ BUL vào comboBox1 (nhập) và comboBox3 (tìm kiếm).</summary>
        private void TaoDanhSachLop()
        {
            List<LopHoc> dsLop = lopBUL.GetAllLopHoc();     // BUL → LopHocDAO → MySQL

            tenLopTheoMa.Clear();
            foreach (LopHoc lh in dsLop)
                tenLopTheoMa[lh.MaLop] = lh.TenLop;

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
        //  TASK 4 – NHẬP / RỜI Ô MÃ SINH VIÊN (txtMa)
        //  • Mã ĐÃ TỒN TẠI  → hiện thông tin sinh viên lên các điều khiển
        //  • Mã CHƯA TỒN TẠI → xóa trống các ô nhập
        // =====================================================================

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {
            if (!daKhoiTao) return;
            erp.SetError(textBox1, string.Empty);
            XuLyMaSinhVien();
        }

        private void textBox1_Leave(object? sender, EventArgs e)
        {
            if (!daKhoiTao) return;
            XuLyMaSinhVien();
        }

        private void XuLyMaSinhVien()
        {
            string maSV = textBox1.Text.Trim();

            if (string.Equals(maSV, maDaXuLy, StringComparison.OrdinalIgnoreCase))
                return;                       // mã chưa đổi → không đụng dữ liệu đang nhập

            maDaXuLy = maSV;

            if (string.IsNullOrEmpty(maSV))   // xóa trắng ô mã → không cần truy vấn
            {
                XoaTrangCacONhap();
                DatTrangThaiNut(maTonTai: false);
                return;
            }

            try
            {
                SinhVien? sv = sinhVienBUL.GetSinhVienByMaSV(maSV);   // ★ BUL → DAO → MySQL

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
            catch (Exception ex)
            {
                HienThiLoiNgoaiLe(ex);
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

            int viTriTrangThai = comboBox2.FindStringExact(sv.TrangThai);
            if (viTriTrangThai >= 0)
                comboBox2.SelectedIndex = viTriTrangThai;
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
            XoaTatCaLoi();

            textBox5.Text = Placeholder;
            textBox5.ForeColor = SystemColors.ScrollBar;
            comboBox3.SelectedIndex = -1;
            numericUpDown2.Value = 0;

            try
            {
                NapBangLenGrid();                 // trả lưới về toàn bộ danh sách
            }
            catch (Exception ex)
            {
                HienThiLoiNgoaiLe(ex);
            }

            DatTrangThaiNut(maTonTai: false);
            textBox1.Focus();                 // ★ trả con trỏ về ô Mã sinh viên
        }

        // =====================================================================
        //  TASK 6 – THÊM / SỬA / XÓA
        //  • Thêm / Sửa: kiểm tra hợp lệ → sai trường nào báo ErrorProvider trường đó
        //  • Sửa / Xóa : bắt buộc hiện hộp thoại xác nhận YesNo
        //  • Mọi ngoại lệ từ BUL / DAO đều được bắt và hiển thị
        // =====================================================================

        /// <summary>NÚT THÊM (button1).</summary>
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                XoaTatCaLoi();

                SinhVien sv = DocDuLieuTuForm();

                // 1. Kiểm tra thông tin hợp lệ → sai trường nào hiện ErrorProvider trường đó
                if (!sv.KiemTraHopLe(out List<ValidationResult> loi))
                {
                    HienThiLoiValidation(loi);
                    return;
                }

                // 2. Gọi tầng Business (Business cũng tự validate lại và ném ValidationException)
                sinhVienBUL.ThemSinhVien(sv);          // ★ Views → BUL → DAO → MySQL

                NapBangLenGrid();
                MessageBox.Show("Thêm sinh viên thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LamMoiNhapLieu();
            }
            catch (ValidationException vex)
            {
                HienThiLoiValidation(vex.Loi);         // lỗi dữ liệu → ErrorProvider
            }
            catch (Exception ex)
            {
                HienThiLoiNgoaiLe(ex);                 // lỗi nghiệp vụ / truy vấn → MessageBox
                DatTrangThaiNut(sinhVienBUL.KiemTraTonTai(textBox1.Text.Trim()));
            }
        }

        /// <summary>NÚT SỬA (button2) – bắt buộc xác nhận YesNo.</summary>
        private void button2_Click(object? sender, EventArgs e)
        {
            try
            {
                XoaTatCaLoi();

                SinhVien svMoi = DocDuLieuTuForm();

                // 1. Kiểm tra hợp lệ TRƯỚC khi hiện hộp thoại xác nhận
                if (!svMoi.KiemTraHopLe(out List<ValidationResult> loi))
                {
                    HienThiLoiValidation(loi);
                    return;
                }

                SinhVien? svCu = sinhVienBUL.GetSinhVienByMaSV(svMoi.MaSV);
                if (svCu == null)
                {
                    MessageBox.Show("Không tìm thấy sinh viên có mã \"" + svMoi.MaSV + "\"!",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    DatTrangThaiNut(false);
                    return;
                }

                // 2. Hộp thoại xác nhận TRƯỚC KHI thực hiện
                DialogResult kq = MessageBox.Show(
                    "Bạn có chắc chắn muốn CẬP NHẬT thông tin của sinh viên\n" +
                    "\"" + svCu.MaSV + " - " + svCu.HoTen + "\" không?",
                    "Xác nhận sửa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (kq != DialogResult.Yes) return;         // chọn No → hủy

                sinhVienBUL.CapNhatSinhVien(svMoi);         // ★ Views → BUL → DAO → MySQL

                NapBangLenGrid();
                MessageBox.Show("Cập nhật thông tin sinh viên thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LamMoiNhapLieu();
            }
            catch (ValidationException vex)
            {
                HienThiLoiValidation(vex.Loi);
            }
            catch (Exception ex)
            {
                HienThiLoiNgoaiLe(ex);
            }
        }

        /// <summary>NÚT XÓA (button3) – bắt buộc xác nhận YesNo.</summary>
        private void button3_Click(object? sender, EventArgs e)
        {
            try
            {
                string maSV = textBox1.Text.Trim();
                SinhVien? sv = sinhVienBUL.GetSinhVienByMaSV(maSV);

                if (sv == null)
                {
                    MessageBox.Show("Không tìm thấy sinh viên cần xóa!",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    DatTrangThaiNut(false);
                    return;
                }

                // Hộp thoại xác nhận TRƯỚC KHI thực hiện
                DialogResult kq = MessageBox.Show(
                    "Bạn có chắc chắn muốn XÓA sinh viên\n" +
                    "\"" + sv.MaSV + " - " + sv.HoTen + "\" không?\n" +
                    "Hành động này không thể hoàn tác.",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (kq != DialogResult.Yes) return;         // chọn No → hủy

                sinhVienBUL.XoaSinhVien(maSV);              // ★ Views → BUL → DAO → MySQL

                NapBangLenGrid();
                MessageBox.Show("Đã xóa sinh viên \"" + sv.MaSV + "\"!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LamMoiNhapLieu();
            }
            catch (Exception ex)
            {
                HienThiLoiNgoaiLe(ex);
            }
        }

        /// <summary>NÚT HIỂN THỊ TẤT CẢ (button5).</summary>
        private void button5_Click(object? sender, EventArgs e)
        {
            textBox5.Text = Placeholder;
            textBox5.ForeColor = SystemColors.ScrollBar;
            comboBox3.SelectedIndex = -1;
            numericUpDown2.Value = 0;

            try
            {
                NapBangLenGrid();
            }
            catch (Exception ex)
            {
                HienThiLoiNgoaiLe(ex);
            }
        }

        // =====================================================================
        //  TÌM KIẾM (tìm kiếm PHÍA CƠ SỞ DỮ LIỆU – WHERE chạy trên MySQL)
        //  Giao diện chỉ nhập tiêu chí rồi gọi BUL, KHÔNG tự lọc trong bộ nhớ
        // =====================================================================

        private void button7_Click(object sender, EventArgs e)
        {
            if (!daKhoiTao) return;

            try
            {
                string tuKhoa = LayTuKhoa();
                string maLop = (comboBox3.SelectedItem as LopHoc)?.MaLop ?? string.Empty;
                double diemToiThieu = (double)numericUpDown2.Value;

                // ★ Tiêu chí được gửi xuống MySQL (SinhVienDAO.TimKiemSinhVien)
                List<SinhVien> dsLoc = sinhVienBUL.TimKiem(tuKhoa, maLop, diemToiThieu);

                NapBangLenGrid(dsLoc);

                if (dsLoc.Count == 0)
                    MessageBox.Show("Không tìm thấy sinh viên phù hợp!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                HienThiLoiNgoaiLe(ex);
            }
        }

        /// <summary>★ Chọn lớp trên comboBox3 → hiển thị sinh viên CỦA LỚP ĐÓ lên lưới.</summary>
        private void comboBox3_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (!daKhoiTao) return;
            if (comboBox3.SelectedItem is not LopHoc lop) return;

            try
            {
                List<SinhVien> dsLop = sinhVienBUL.GetSinhVienByMaLop(lop.MaLop);
                NapBangLenGrid(dsLop);
            }
            catch (Exception ex)
            {
                HienThiLoiNgoaiLe(ex);
            }
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
        //  CHỌN DÒNG TRÊN dataGridView1
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

            try
            {
                string maSV = dtBang.Rows[rowIndex]["MaSV"].ToString() ?? string.Empty;
                SinhVien? sv = sinhVienBUL.GetSinhVienByMaSV(maSV);
                if (sv == null) return;

                DoDuLieuLenForm(sv);
                maDaXuLy = sv.MaSV;
                DatTrangThaiNut(maTonTai: true);
            }
            catch (Exception ex)
            {
                HienThiLoiNgoaiLe(ex);
            }
        }

        // =====================================================================
        //  ERRORPROVIDER – BÁO LỖI TỪNG TRƯỜNG
        // =====================================================================

        /// <summary>Hiển thị lỗi lên ErrorProvider đúng trường + focus vào ô lỗi đầu tiên.</summary>
        private void HienThiLoiValidation(List<ValidationResult> loi)
        {
            Control? oDauTien = null;
            List<string> loiChuaGan = new List<string>();

            foreach (ValidationResult loiItem in loi)
            {
                string tenTruong = loiItem.MemberNames != null && loiItem.MemberNames.Any()
                    ? loiItem.MemberNames.First()
                    : string.Empty;
                string thongDiep = loiItem.ErrorMessage ?? "Dữ liệu không hợp lệ.";

                switch (tenTruong)
                {
                    case "MaSV":
                        erp.SetError(textBox1, thongDiep);
                        oDauTien ??= textBox1;
                        break;
                    case "HoTen":
                        erp.SetError(textBox4, thongDiep);
                        oDauTien ??= textBox4;
                        break;
                    case "NgaySinh":
                        erp.SetError(dateTimePicker1, thongDiep);
                        oDauTien ??= dateTimePicker1;
                        break;
                    case "GioiTinh":
                        erp.SetError(radioButton1, thongDiep);
                        erp.SetError(radioButton2, thongDiep);
                        oDauTien ??= radioButton1;
                        break;
                    case "DienThoai":
                        erp.SetError(textBox2, thongDiep);
                        oDauTien ??= textBox2;
                        break;
                    case "Email":
                        erp.SetError(textBox3, thongDiep);
                        oDauTien ??= textBox3;
                        break;
                    case "MaLop":
                        erp.SetError(comboBox1, thongDiep);
                        oDauTien ??= comboBox1;
                        break;
                    case "Diem":
                        erp.SetError(numericUpDown1, thongDiep);
                        oDauTien ??= numericUpDown1;
                        break;
                    case "TrangThai":
                        erp.SetError(comboBox2, thongDiep);
                        oDauTien ??= comboBox2;
                        break;
                    default:
                        loiChuaGan.Add(thongDiep);
                        break;
                }
            }

            if (oDauTien != null)
            {
                oDauTien.Focus();
            }
            else if (loiChuaGan.Count > 0)
            {
                MessageBox.Show(string.Join("\n", loiChuaGan), "Lỗi dữ liệu",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>Xóa toàn bộ dấu lỗi ErrorProvider trên form.</summary>
        private void XoaTatCaLoi()
        {
            foreach (Control c in new Control[]
                     { textBox1, textBox4, dateTimePicker1, radioButton1, radioButton2,
                       textBox2, textBox3, comboBox1, numericUpDown1, comboBox2 })
            {
                erp.SetError(c, string.Empty);
            }
        }

        /// <summary>Bắt ngoại lệ chung từ BUL / DAO và hiển thị cho người dùng.</summary>
        private void HienThiLoiNgoaiLe(Exception ex)
        {
            string tieuDe = ex switch
            {
                BusinessException => "Lỗi nghiệp vụ",
                DataAccessException => "Lỗi cơ sở dữ liệu",
                _ => "Thông báo lỗi"
            };

            MessageBox.Show(ex.Message, tieuDe,
                MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        /// <summary>Lấy tên lớp hiển thị trên lưới (tra trong Dictionary, không truy vấn lại DB).</summary>
        private string TenLopCua(string maLop)
        {
            return tenLopTheoMa.TryGetValue(maLop, out string? ten) ? ten : maLop;
        }

        /// <summary>Nạp dữ liệu (mặc định lấy toàn bộ từ BUL) lên dataGridView1.</summary>
        private void NapBangLenGrid(IEnumerable<SinhVien>? dsHienThi = null)
        {
            dtBang.Rows.Clear();

            IEnumerable<SinhVien> ds =
                dsHienThi ?? sinhVienBUL.GetAllSinhVien();      // ★ BUL → DAO → MySQL

            foreach (SinhVien sv in ds)
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
