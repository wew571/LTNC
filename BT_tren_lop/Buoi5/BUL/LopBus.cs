using System;
using System.Collections.Generic;
using ProjectQLsinhvien.Data.DAL;
using ProjectQLsinhvien.Entity;

namespace ProjectQLsinhvien.BUL
{
    /// <summary>
    /// TẦNG BUL (Business Logic Layer) – NGHIỆP VỤ LỚP HỌC.
    /// Nhiệm vụ: kiểm tra quy tắc nghiệp vụ rồi MỚI gọi DAL ghi dữ liệu.
    /// - KHÔNG truy cập CSDL trực tiếp (phải qua LopDAL).
    /// - KHÔNG vẽ giao diện / MessageBox (đó là việc của Form).
    /// </summary>
    public class LopBus
    {
        private readonly LopDAL lopDAL;

        public LopBus()
        {
            lopDAL = new LopDAL();
        }

        // ===== Form gọi các hàm này =====

        /// <summary>Lấy toàn bộ lớp (dùng nạp comboBox1 / comboBox3).</summary>
        public List<LopHoc> GetAllLopHoc()
        {
            return lopDAL.GetAllLopHoc();
        }

        /// <summary>Tra cứu 1 lớp theo mã – trả null nếu không có.</summary>
        public LopHoc? GetLopById(string maLop)
        {
            return lopDAL.GetLopById(maLop);
        }

        /// <summary>Kiểm tra mã lớp đã tồn tại chưa.</summary>
        public bool KiemTraTonTai(string maLop)
        {
            return lopDAL.IsExist(maLop);
        }

        /// <summary>
        /// Thêm lớp: Validate (Entity) → Chống trùng mã (Business) → Ghi (DAL).
        /// Ném Exception nếu vi phạm – Form sẽ bắt và hiện MessageBox.
        /// </summary>
        public void ThemLopHoc(LopHoc lh)
        {
            // 1. Rule nghiệp vụ 1: dữ liệu phải hợp lệ (Data Annotation của Entity)
            List<string> loi = lh.IsInValid();
            if (loi.Count > 0)
                throw new Exception("Lớp học không hợp lệ:\n" +
                                    string.Join("\n", loi.ConvertAll(l => "- " + l)));

            // 2. Rule nghiệp vụ 2: không được trùng mã lớp
            if (lopDAL.IsExist(lh.MaLop))
                throw new Exception("Mã lớp \"" + lh.MaLop + "\" đã tồn tại.");

            // 3. Ghi xuống DAL
            lopDAL.AddLopHoc(lh);
        }

        /// <summary>Cập nhật lớp theo mã.</summary>
        public void CapNhatLopHoc(LopHoc lh)
        {
            List<string> loi = lh.IsInValid();
            if (loi.Count > 0)
                throw new Exception("Lớp học không hợp lệ:\n" +
                                    string.Join("\n", loi.ConvertAll(l => "- " + l)));

            if (!lopDAL.UpdateLopHoc(lh))
                throw new Exception("Không tìm thấy lớp có mã \"" + lh.MaLop + "\".");
        }

        /// <summary>Xóa lớp theo mã.</summary>
        public void XoaLopHoc(string maLop)
        {
            if (!lopDAL.IsExist(maLop))
                throw new Exception("Không tìm thấy lớp có mã \"" + maLop + "\".");

            // Rule nghiệp vụ: không được xóa lớp đang còn sinh viên
            LopHoc? lh = lopDAL.GetLopById(maLop);
            if (lh != null && lh.DanhSachSinhVien.Count > 0)
                throw new Exception("Không thể xóa lớp \"" + maLop +
                                    "\" vì còn " + lh.DanhSachSinhVien.Count + " sinh viên.");

            lopDAL.DeleteLopHoc(maLop);
        }
    }
}
