using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ProjectQLsinhvien.Common;
using ProjectQLsinhvien.Data.DAO;
using ProjectQLsinhvien.Entity;
using ValidationException = ProjectQLsinhvien.Common.ValidationException;

namespace ProjectQLsinhvien.BUL
{
    /// <summary>
    /// TẦNG BUL (Business Logic Layer) – NGHIỆP VỤ SINH VIÊN.
    /// Nhận yêu cầu từ Views → kiểm tra nghiệp vụ → gọi SinhVienDAO
    /// để đọc/ghi cơ sở dữ liệu MySQL.
    /// Mọi trường hợp sai đều NÉM NGOẠI LỆ để Views bắt và hiển thị:
    ///     + ValidationException → lỗi dữ liệu (Views gắn ErrorProvider từng trường)
    ///     + BusinessException  → lỗi nghiệp vụ (Views hiện MessageBox)
    ///     + DataAccessException → lỗi truy vấn MySQL (Views hiện MessageBox)
    /// </summary>
    public class SinhVienBUL
    {
        private readonly SinhVienDAO svDAO = new SinhVienDAO();

        // ===== Views gọi các hàm này =====

        /// <summary>Lấy toàn bộ sinh viên (nạp dataGridView).</summary>
        public List<SinhVien> GetAllSinhVien()
        {
            return svDAO.GetAllSinhVien();
        }

        /// <summary>Lấy danh sách sinh viên CỦA MỘT LỚP – dùng khi chọn lớp trên comboBox.</summary>
        public List<SinhVien> GetSinhVienByMaLop(string maLop)
        {
            if (string.IsNullOrWhiteSpace(maLop))
                throw new BusinessException("Vui lòng chọn lớp cần xem danh sách.");

            return svDAO.GetSinhVienByMaLop(maLop);
        }

        /// <summary>Tra cứu sinh viên theo mã – trả null nếu không tồn tại.
        /// Dùng cho Task 4 (nhập/rời ô Mã sinh viên).</summary>
        public SinhVien? GetSinhVienByMaSV(string maSV)
        {
            return svDAO.GetSinhVienByMaSV(maSV);
        }

        /// <summary>Kiểm tra mã sinh viên đã tồn tại trong CSDL chưa.</summary>
        public bool KiemTraTonTai(string maSV)
        {
            return svDAO.IsExist(maSV);
        }

        /// <summary>
        /// TÌM KIẾM PHÍA CƠ SỞ DỮ LIỆU: từ khóa, lớp, điểm tối thiểu được gửi
        /// xuống MySQL qua WHERE (parameter), CSDL trả về đúng kết quả phù hợp.
        /// </summary>
        public List<SinhVien> TimKiem(string tuKhoa, string maLop, double diemToiThieu)
        {
            if (diemToiThieu < 0 || diemToiThieu > 10)
                throw new BusinessException("Điểm tối thiểu phải nằm trong khoảng từ 0 đến 10.");

            return svDAO.TimKiemSinhVien(tuKhoa ?? string.Empty, maLop ?? string.Empty, diemToiThieu);
        }

        /// <summary>
        /// THÊM sinh viên:
        /// Validate (Entity) → Chống trùng mã (Business) → Lớp phải tồn tại → Ghi (DAL).
        /// </summary>
        public void ThemSinhVien(SinhVien sv)
        {
            // 1. Rule nghiệp vụ 1: dữ liệu hợp lệ theo Data Annotation của Entity
            List<ValidationResult> loi = sv.IsInValid();
            if (loi.Count > 0)
                throw new ValidationException(loi);

            // 2. Rule nghiệp vụ 2: không được trùng mã sinh viên
            if (svDAO.IsExist(sv.MaSV))
                throw new BusinessException("Mã sinh viên \"" + sv.MaSV + "\" đã tồn tại.");

            // 3. Rule nghiệp vụ 3 (quan hệ 1 - nhiều): lớp phải tồn tại
            if (!KiemTraLopTonTai(sv.MaLop))
                throw new BusinessException("Lớp học có mã \"" + sv.MaLop + "\" không tồn tại.");

            // 4. Ghi xuống DAL
            svDAO.InsertSinhVien(sv);
        }

        /// <summary>CẬP NHẬT sinh viên (Views sẽ hiện hộp thoại YesNo trước khi gọi).</summary>
        public void CapNhatSinhVien(SinhVien sv)
        {
            List<ValidationResult> loi = sv.IsInValid();
            if (loi.Count > 0)
                throw new ValidationException(loi);

            if (!svDAO.IsExist(sv.MaSV))
                throw new BusinessException("Không tìm thấy sinh viên có mã \"" + sv.MaSV + "\".");

            if (!KiemTraLopTonTai(sv.MaLop))
                throw new BusinessException("Lớp học có mã \"" + sv.MaLop + "\" không tồn tại.");

            if (!svDAO.UpdateSinhVien(sv))
                throw new BusinessException("Cập nhật sinh viên thất bại, vui lòng thử lại.");
        }

        /// <summary>XÓA sinh viên theo mã (Views sẽ hiện hộp thoại YesNo trước khi gọi).</summary>
        public void XoaSinhVien(string maSV)
        {
            if (!svDAO.IsExist(maSV))
                throw new BusinessException("Không tìm thấy sinh viên có mã \"" + maSV + "\".");

            if (!svDAO.DeleteSinhVien(maSV))
                throw new BusinessException("Xóa sinh viên thất bại, vui lòng thử lại.");
        }

        // ==================== HỖ TRỢ NGHIỆP VỤ ====================

        /// <summary>Kiểm tra lớp có tồn tại không (quan hệ 1 - nhiều) – nhờ LopBUL.</summary>
        private bool KiemTraLopTonTai(string maLop)
        {
            if (string.IsNullOrWhiteSpace(maLop)) return false;
            return new LopBUL().KiemTraTonTai(maLop);
        }
    }
}
