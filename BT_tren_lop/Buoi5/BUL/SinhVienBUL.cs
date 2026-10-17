using System;
using System.Collections.Generic;
using System.Linq;
using ProjectQLsinhvien.Data.DAL;
using ProjectQLsinhvien.Entity;

namespace ProjectQLsinhvien.BUL
{
    /// <summary>
    /// TẦNG BUL (Business Logic Layer) – NGHIỆP VỤ SINH VIÊN.
    /// Nhận dữ liệu từ Form (Views) → kiểm tra nghiệp vụ → gọi SinhVienDAL1 ghi.
    /// Ném Exception khi vi phạm để Form bắt và hiện MessageBox.
    /// </summary>
    public class SinhVienBUL
    {
        private readonly SinhVienDAL1 svd;

        /// <summary>Dùng để kiểm tra lớp có tồn tại (quan hệ 1 - nhiều).</summary>
        private readonly LopBus lopBus;

        public SinhVienBUL()
        {
            svd = new SinhVienDAL1();
            lopBus = new LopBus();
        }

        // ===== Form gọi các hàm này =====

        /// <summary>Lấy toàn bộ sinh viên (nạp dataGridView1).</summary>
        public List<SinhVien> GetAllSinhVien()
        {
            return svd.GetAllSinhVien();
        }

        /// <summary>Tra cứu sinh viên theo mã – trả null nếu không tồn tại.
        /// Dùng cho Task 4 (nhập/rời ô Mã sinh viên).</summary>
        public SinhVien? GetSinhVienById(string maSV)
        {
            return svd.GetSinhVienById(maSV);
        }

        /// <summary>Kiểm tra mã sinh viên đã tồn tại (Task 4).</summary>
        public bool KiemTraTonTai(string maSV)
        {
            return svd.IsExist(maSV);
        }

        /// <summary>
        /// THÊM sinh viên:
        /// Validate (Entity) → Chống trùng mã (Business) → Ghi (DAL).
        /// </summary>
        public void ThemSinhVien(SinhVien sv)
        {
            // 1. Rule nghiệp vụ 1: dữ liệu hợp lệ theo Data Annotation của Entity
            if (sv.IsInValid().Count > 0)
            {
                List<string> loi = sv.IsInValid();
                throw new Exception("Sinh viên không hợp lệ:\n" +
                                    string.Join("\n", loi.ConvertAll(l => "- " + l)));
            }

            // 2. Rule nghiệp vụ 2: không được trùng mã sinh viên
            if (svd.GetSinhVienById(sv.MaSV) != null)
            {
                throw new Exception("Mã sinh viên đã tồn tại");
            }

            // 3. Rule nghiệp vụ 3 (quan hệ 1 - nhiều): lớp phải tồn tại
            if (lopBus.GetLopById(sv.MaLop) == null)
            {
                throw new Exception("Lớp học có mã \"" + sv.MaLop + "\" không tồn tại.");
            }

            // 4. Ghi xuống DAL
            svd.AddSinhVien(sv);
        }

        /// <summary>CẬP NHẬT sinh viên (Form sẽ hiện hộp thoại YesNo trước khi gọi).</summary>
        public void CapNhatSinhVien(SinhVien sv)
        {
            List<string> loi = sv.IsInValid();
            if (loi.Count > 0)
                throw new Exception("Sinh viên không hợp lệ:\n" +
                                    string.Join("\n", loi.ConvertAll(l => "- " + l)));

            if (!svd.IsExist(sv.MaSV))
                throw new Exception("Không tìm thấy sinh viên có mã \"" + sv.MaSV + "\".");

            if (lopBus.GetLopById(sv.MaLop) == null)
                throw new Exception("Lớp học có mã \"" + sv.MaLop + "\" không tồn tại.");

            if (!svd.UpdateSinhVien(sv))
                throw new Exception("Cập nhật thất bại, vui lòng thử lại.");
        }

        /// <summary>XÓA sinh viên theo mã (Form sẽ hiện hộp thoại YesNo trước khi gọi).</summary>
        public void XoaSinhVien(string maSV)
        {
            if (!svd.IsExist(maSV))
                throw new Exception("Không tìm thấy sinh viên có mã \"" + maSV + "\".");

            if (!svd.DeleteSinhVien(maSV))
                throw new Exception("Xóa thất bại, vui lòng thử lại.");
        }

        /// <summary>
        /// TÌM KIẾM: rule lọc dữ liệu – nằm ở BUL vì đây là nghiệp vụ,
        /// Form chỉ truyền từ khóa vào rồi nhận danh sách về.
        /// </summary>
        public List<SinhVien> TimKiem(string tuKhoa, string? maLop, double diemToiThieu)
        {
            string tk = (tuKhoa ?? string.Empty).Trim().ToLower();

            return svd.GetAllSinhVien().Where(sv =>
                    (tk.Length == 0 ||
                     (sv.MaSV ?? string.Empty).ToLower().Contains(tk) ||
                     (sv.HoTen ?? string.Empty).ToLower().Contains(tk) ||
                     (sv.Email ?? string.Empty).ToLower().Contains(tk))
                    && (string.IsNullOrEmpty(maLop) || sv.MaLop == maLop)
                    && sv.Diem >= diemToiThieu)
                .ToList();
        }
    }
}
