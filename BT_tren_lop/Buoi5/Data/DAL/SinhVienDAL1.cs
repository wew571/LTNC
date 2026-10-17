using System.Collections.Generic;
using System.Linq;
using ProjectQLsinhvien.Entity;

namespace ProjectQLsinhvien.Data.DAL
{
    /// <summary>
    /// TẦNG DAL (Data Access Layer) – SINH VIÊN.
    /// Nhiệm vụ: ĐỌC / GHI dữ liệu thuần túy + giữ toàn vẹn tham chiếu
    /// (quan hệ 1 lớp – nhiều sinh viên) trong "CSDL".
    /// Không validate nghiệp vụ – đó là việc của BUL.
    /// </summary>
    public class SinhVienDAL1
    {
        /// <summary>"BẢNG sinh viên" – static để mọi DAL thấy chung một dữ liệu.</summary>
        private static readonly List<SinhVien> sinhViens = new List<SinhVien>();

        private static bool daKhoiTao = false;

        /// <summary>Dùng để nối sinh viên vào lớp tương ứng (quan hệ 1 - nhiều).</summary>
        private static readonly LopDAL lopDAL = new LopDAL();

        public SinhVienDAL1()
        {
            KhoiTaoDuLieuMau();
        }

        /// <summary>Nạp dữ liệu mẫu cho "bảng sinh viên".</summary>
        private static void KhoiTaoDuLieuMau()
        {
            if (daKhoiTao) return;

            ThemVaoBang(new SinhVien
            {
                MaSV = "SV001", HoTen = "Nguyễn Văn An",
                NgaySinh = new System.DateTime(2005, 1, 15), GioiTinh = "Nam",
                DienThoai = "0901234567", Email = "an.nguyenvan@gmail.com",
                MaLop = "CSE0001", Diem = 7.5, TrangThai = "Đang học"
            });

            ThemVaoBang(new SinhVien
            {
                MaSV = "SV002", HoTen = "Trần Thị Bình",
                NgaySinh = new System.DateTime(2005, 3, 22), GioiTinh = "Nữ",
                DienThoai = "0912345678", Email = "binh.tranthi@gmail.com",
                MaLop = "CSE0002", Diem = 8.2, TrangThai = "Đang học"
            });

            ThemVaoBang(new SinhVien
            {
                MaSV = "SV003", HoTen = "Lê Văn Cường",
                NgaySinh = new System.DateTime(2004, 11, 5), GioiTinh = "Nam",
                DienThoai = "0923456781", Email = "cuong.levan@gmail.com",
                MaLop = "CSE0001", Diem = 6.4, TrangThai = "Bảo lưu"
            });

            ThemVaoBang(new SinhVien
            {
                MaSV = "SV004", HoTen = "Phạm Thu Hà",
                NgaySinh = new System.DateTime(2005, 7, 9), GioiTinh = "Nữ",
                DienThoai = "0934567892", Email = "ha.phamthu@gmail.com",
                MaLop = "CSE0003", Diem = 9.1, TrangThai = "Đã tốt nghiệp"
            });

            daKhoiTao = true;
        }

        // ==================== CÁC THAO TÁC VỚI DỮ LIỆU ====================

        /// <summary>Lấy toàn bộ sinh viên (dùng để nạp lên dataGridView1).</summary>
        public List<SinhVien> GetAllSinhVien()
        {
            return sinhViens.ToList();   // bản sao danh sách, phần tử là tham chiếu dùng chung
        }

        /// <summary>Tìm sinh viên theo mã – trả null nếu không tồn tại.</summary>
        public SinhVien? GetSinhVienById(string maSV)
        {
            if (string.IsNullOrWhiteSpace(maSV)) return null;
            return sinhViens.FirstOrDefault(sv => sv.MaSV == maSV);
        }

        /// <summary>Kiểm tra mã sinh viên đã tồn tại chưa.</summary>
        public bool IsExist(string maSV)
        {
            return GetSinhVienById(maSV) != null;
        }

        /// <summary>Thêm 1 sinh viên vào "bảng".</summary>
        public void AddSinhVien(SinhVien sv)
        {
            ThemVaoBang(sv);
        }

        /// <summary>Cập nhật sinh viên (thay bản ghi theo mã). Trả về false nếu không thấy.</summary>
        public bool UpdateSinhVien(SinhVien sv)
        {
            int viTri = sinhViens.FindIndex(s => s.MaSV == sv.MaSV);
            if (viTri < 0) return false;

            SinhVien cu = sinhViens[viTri];

            // Gỡ khỏi lớp CŨ trước khi thay bản ghi (không để sót tham chiếu)
            GoKhoiLop(cu);

            sinhViens[viTri] = sv;

            // Nối vào lớp MỚI (theo MaLop trên bản ghi vừa cập nhật)
            NoiVaoLop(sv);
            return true;
        }

        /// <summary>Xóa 1 sinh viên theo mã. Trả về false nếu không thấy.</summary>
        public bool DeleteSinhVien(string maSV)
        {
            SinhVien? sv = GetSinhVienById(maSV);
            if (sv == null) return false;

            GoKhoiLop(sv);                 // gỡ khỏi quan hệ 1 - nhiều
            return sinhViens.Remove(sv);
        }

        // ==================== GIỮ TOÀN VẸN QUAN HỆ 1 - NHIỀU ====================

        /// <summary>Đưa sinh viên vào danh sách của lớp tương ứng.</summary>
        private static void NoiVaoLop(SinhVien sv)
        {
            LopHoc? lop = lopDAL.GetLopById(sv.MaLop);
            sv.Lop = lop;
            if (lop != null && !lop.DanhSachSinhVien.Contains(sv))
                lop.DanhSachSinhVien.Add(sv);
        }

        /// <summary>Gỡ sinh viên khỏi lớp mà nó đang thuộc về.</summary>
        private static void GoKhoiLop(SinhVien sv)
        {
            sv.Lop?.DanhSachSinhVien.Remove(sv);
            sv.Lop = null;
        }

        /// <summary>Thao tác ghi dùng chung cho Add và seed dữ liệu mẫu.</summary>
        private static void ThemVaoBang(SinhVien sv)
        {
            sinhViens.Add(sv);
            NoiVaoLop(sv);
        }
    }
}
