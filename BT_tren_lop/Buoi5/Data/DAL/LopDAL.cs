using System.Collections.Generic;
using System.Linq;
using ProjectQLsinhvien.Entity;

namespace ProjectQLsinhvien.Data.DAL
{
    /// <summary>
    /// TẦNG DAL (Data Access Layer) – LỚP HỌC.
    /// Nhiệm vụ: ĐỌC / GHI dữ liệu thuần túy.
    /// - Không biết Business Rule (không validate nghiệp vụ).
    /// - Không biết Giao diện (không gọi Form, MessageBox).
    /// Dữ liệu đang lưu "trong bộ nhớ" (List) đóng vai trò bảng trong CSDL;
    /// sau này chỉ cần viết lại bên trong file này là chuyển sang SQL Server,
    /// không phải đụng tới BUL hay Views.
    /// </summary>
    public class LopDAL
    {
        /// <summary>
        /// "BẢNG lớp học" trong CSDL – static để MỌI lần new LopDAL()
        /// đều thấy CÙNG một danh sách (giống mọi kết nối DB trỏ chung 1 database).
        /// </summary>
        private static readonly List<LopHoc> lopHocs = new List<LopHoc>();

        /// <summary>Cờ seed dữ liệu mẫu – chỉ thực hiện 1 lần duy nhất.</summary>
        private static bool daKhoiTao = false;

        public LopDAL()
        {
            KhoiTaoDuLieuMau();
        }

        /// <summary>Nạp dữ liệu mẫu cho "bảng lớp học".</summary>
        private static void KhoiTaoDuLieuMau()
        {
            if (daKhoiTao) return;

            lopHocs.Add(new LopHoc { MaLop = "CSE0001", TenLop = "Khoa học máy tính 01" });
            lopHocs.Add(new LopHoc { MaLop = "CSE0002", TenLop = "Khoa học máy tính 02" });
            lopHocs.Add(new LopHoc { MaLop = "CSE0003", TenLop = "Khoa học máy tính 03" });

            daKhoiTao = true;
        }

        // ==================== CÁC THAO TÁC VỚI DỮ LIỆU ====================

        /// <summary>Lấy toàn bộ lớp học.</summary>
        public List<LopHoc> GetAllLopHoc()
        {
            return lopHocs.ToList();   // bản sao danh sách (phần tử vẫn là tham chiếu dùng chung)
        }

        /// <summary>Tìm 1 lớp theo mã – trả null nếu không có.</summary>
        public LopHoc? GetLopById(string maLop)
        {
            if (string.IsNullOrWhiteSpace(maLop)) return null;
            return lopHocs.FirstOrDefault(l => l.MaLop == maLop);
        }

        /// <summary>Kiểm tra mã lớp đã tồn tại chưa.</summary>
        public bool IsExist(string maLop)
        {
            return GetLopById(maLop) != null;
        }

        /// <summary>Thêm 1 lớp vào "bảng".</summary>
        public void AddLopHoc(LopHoc lh)
        {
            lopHocs.Add(lh);
        }

        /// <summary>Cập nhật 1 lớp theo mã. Trả về false nếu không tìm thấy.</summary>
        public bool UpdateLopHoc(LopHoc lh)
        {
            LopHoc? cu = GetLopById(lh.MaLop);
            if (cu == null) return false;

            cu.TenLop = lh.TenLop;
            cu.GhiChu = lh.GhiChu;
            return true;
        }

        /// <summary>Xóa 1 lớp theo mã. Trả về false nếu không tìm thấy.</summary>
        public bool DeleteLopHoc(string maLop)
        {
            LopHoc? lh = GetLopById(maLop);
            if (lh == null) return false;
            return lopHocs.Remove(lh);
        }
    }
}
