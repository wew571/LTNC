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
    /// TẦNG BUL (Business Logic Layer) – NGHIỆP VỤ LỚP HỌC.
    /// Nhận yêu cầu từ Views → kiểm tra quy tắc nghiệp vụ → gọi LopHocDAO
    /// để đọc/ghi cơ sở dữ liệu MySQL.
    /// - KHÔNG truy cập CSDL trực tiếp (phải qua LopHocDAO).
    /// - KHÔNG vẽ giao diện / MessageBox (đó là việc của Form).
    /// - Mọi trường hợp sai đều NÉM NGOẠI LỆ để Views bắt và hiển thị:
    ///     + ValidationException → lỗi dữ liệu (Views gắn ErrorProvider từng trường)
    ///     + BusinessException  → lỗi nghiệp vụ (Views hiện MessageBox)
    ///     + DataAccessException → lỗi truy vấn MySQL (Views hiện MessageBox)
    /// </summary>
    public class LopBUL
    {
        private readonly LopHocDAO lopDAO = new LopHocDAO();

        // ===== Views gọi các hàm này =====

        /// <summary>Lấy toàn bộ lớp (nạp comboBox).</summary>
        public List<LopHoc> GetAllLopHoc()
        {
            return lopDAO.GetAllLopHoc();
        }

        /// <summary>Tra cứu 1 lớp theo mã – trả null nếu không có.</summary>
        public LopHoc? GetLopByMaLop(string maLop)
        {
            return lopDAO.GetLopByMaLop(maLop);
        }

        /// <summary>Kiểm tra mã lớp đã tồn tại trong CSDL chưa.</summary>
        public bool KiemTraTonTai(string maLop)
        {
            return lopDAO.IsExist(maLop);
        }

        /// <summary>Tìm lớp theo từ khóa (tìm kiếm phía cơ sở dữ liệu).</summary>
        public List<LopHoc> TimKiemLop(string tuKhoa)
        {
            return lopDAO.TimKiemLop(tuKhoa);
        }

        /// <summary>
        /// THÊM lớp: Validate (Entity) → Chống trùng mã (Business) → Ghi (DAL).
        /// Ném ValidationException / BusinessException nếu vi phạm.
        /// </summary>
        public void ThemLopHoc(LopHoc lh)
        {
            // 1. Rule nghiệp vụ 1: dữ liệu phải hợp lệ (Data Annotation của Entity)
            List<ValidationResult> loi = lh.IsInValid();
            if (loi.Count > 0)
                throw new ValidationException(loi);

            // 2. Rule nghiệp vụ 2: không được trùng mã lớp
            if (lopDAO.IsExist(lh.MaLop))
                throw new BusinessException("Mã lớp \"" + lh.MaLop + "\" đã tồn tại.");

            // 3. Ghi xuống DAL
            lopDAO.InsertLopHoc(lh);
        }

        /// <summary>Cập nhật lớp theo mã.</summary>
        public void CapNhatLopHoc(LopHoc lh)
        {
            List<ValidationResult> loi = lh.IsInValid();
            if (loi.Count > 0)
                throw new ValidationException(loi);

            if (!lopDAO.IsExist(lh.MaLop))
                throw new BusinessException("Không tìm thấy lớp có mã \"" + lh.MaLop + "\".");

            if (!lopDAO.UpdateLopHoc(lh))
                throw new BusinessException("Cập nhật lớp thất bại, vui lòng thử lại.");
        }

        /// <summary>Xóa lớp theo mã – không cho xóa lớp đang còn sinh viên.</summary>
        public void XoaLopHoc(string maLop)
        {
            if (!lopDAO.IsExist(maLop))
                throw new BusinessException("Không tìm thấy lớp có mã \"" + maLop + "\".");

            // Rule nghiệp vụ: không được xóa lớp đang còn sinh viên (quan hệ 1 - nhiều)
            int soSV = lopDAO.DemSinhVien(maLop);
            if (soSV > 0)
                throw new BusinessException("Không thể xóa lớp \"" + maLop +
                                            "\" vì còn " + soSV + " sinh viên.");

            lopDAO.DeleteLopHoc(maLop);
        }
    }
}
