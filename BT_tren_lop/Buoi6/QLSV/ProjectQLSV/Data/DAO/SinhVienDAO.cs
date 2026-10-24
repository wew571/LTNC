using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using ProjectQLsinhvien.Common;
using ProjectQLsinhvien.Data.DB;
using ProjectQLsinhvien.Entity;

namespace ProjectQLsinhvien.Data.DAO
{
    /// <summary>
    /// TẦNG DAL (Data Access Layer) – SINH VIÊN.
    /// Mỗi thực thể có 1 lớp DAO riêng; lớp này chỉ THUẦN thao tác với bảng SinhVien
    /// trên MySQL bằng ADO.NET:
    ///   • Thêm  → InsertSinhVien
    ///   • Sửa   → UpdateSinhVien
    ///   • Xóa   → DeleteSinhVien
    ///   • Lấy về DataList → GetAllSinhVien / GetSinhVienByMaSV / GetSinhVienByMaLop / TimKiem
    /// Không validate nghiệp vụ (việc của BUL), không đụng giao diện (việc của Views).
    /// Mọi lỗi truy vấn đều ném DataAccessException.
    /// </summary>
    public class SinhVienDAO
    {
        private readonly MySqlDbHelper db = new MySqlDbHelper();

        // ==================== LẤY VỀ (SELECT) ====================

        /// <summary>Lấy toàn bộ sinh viên – trả về DataList&lt;SinhVien&gt;.</summary>
        public List<SinhVien> GetAllSinhVien()
        {
            return DocDanhSach(CauLenhChon + " ORDER BY MaSV");
        }

        /// <summary>Lấy danh sách sinh viên CỦA MỘT LỚP (dùng khi chọn lớp trên comboBox).</summary>
        public List<SinhVien> GetSinhVienByMaLop(string maLop)
        {
            return DocDanhSach(
                CauLenhChon + " WHERE MaLop = @maLop ORDER BY MaSV",
                MySqlDbHelper.TaoThamSo("@maLop", maLop));
        }

        /// <summary>Tìm sinh viên theo mã – trả về null nếu không tồn tại.</summary>
        public SinhVien? GetSinhVienByMaSV(string maSV)
        {
            if (string.IsNullOrWhiteSpace(maSV)) return null;

            List<SinhVien> ds = DocDanhSach(
                CauLenhChon + " WHERE MaSV = @maSV",
                MySqlDbHelper.TaoThamSo("@maSV", maSV.Trim()));

            return ds.Count > 0 ? ds[0] : null;
        }

        /// <summary>Kiểm tra mã sinh viên đã tồn tại trong CSDL chưa.</summary>
        public bool IsExist(string maSV)
        {
            if (string.IsNullOrWhiteSpace(maSV)) return false;

            object ketQua = db.ExecuteScalar(
                "SELECT COUNT(*) FROM SinhVien WHERE MaSV = @maSV",
                MySqlDbHelper.TaoThamSo("@maSV", maSV.Trim()));

            return Convert.ToInt32(ketQua) > 0;
        }

        /// <summary>Đếm số sinh viên của 1 lớp (dùng kiểm tra trước khi xóa lớp).</summary>
        public int DemSinhVien(string maLop)
        {
            object ketQua = db.ExecuteScalar(
                "SELECT COUNT(*) FROM SinhVien WHERE MaLop = @maLop",
                MySqlDbHelper.TaoThamSo("@maLop", maLop));

            return Convert.ToInt32(ketQua);
        }

        /// <summary>
        /// TÌM KIẾM PHÍA CƠ SỞ DỮ LIỆU: điều kiện WHERE được gửi xuống MySQL,
        /// CSDL chỉ trả về những dòng phù hợp (không tải hết bảng về máy).
        /// </summary>
        public List<SinhVien> TimKiemSinhVien(string tuKhoa, string maLop, double diemToiThieu)
        {
            string tk = (tuKhoa ?? string.Empty).Trim();
            string like = "%" + Escape(tk) + "%";

            return DocDanhSach(
                @"SELECT MaSV, HoTen, NgaySinh, GioiTinh, DienThoai, Email, MaLop, Diem, TrangThai
                  FROM SinhVien
                  WHERE (@tk = '' OR MaSV LIKE @like OR HoTen LIKE @like OR Email LIKE @like)
                    AND (@maLop = '' OR MaLop = @maLop)
                    AND Diem >= @diem
                  ORDER BY MaSV",
                MySqlDbHelper.TaoThamSo("@tk", tk),
                MySqlDbHelper.TaoThamSo("@like", like),
                MySqlDbHelper.TaoThamSo("@maLop", maLop ?? string.Empty),
                MySqlDbHelper.TaoThamSo("@diem", diemToiThieu));
        }

        // ==================== THÊM / SỬA / XÓA ====================

        /// <summary>Thêm 1 sinh viên (INSERT). Ném DataAccessException nếu lỗi.</summary>
        public void InsertSinhVien(SinhVien sv)
        {
            db.ExecuteNonQuery(
                @"INSERT INTO SinhVien
                      (MaSV, HoTen, NgaySinh, GioiTinh, DienThoai, Email, MaLop, Diem, TrangThai)
                  VALUES
                      (@maSV, @hoTen, @ngaySinh, @gioiTinh, @dienThoai, @email, @maLop, @diem, @trangThai)",
                MySqlDbHelper.TaoThamSo("@maSV", sv.MaSV),
                MySqlDbHelper.TaoThamSo("@hoTen", sv.HoTen),
                new MySqlParameter("@ngaySinh", MySqlDbType.Date) { Value = sv.NgaySinh.Date },
                MySqlDbHelper.TaoThamSo("@gioiTinh", sv.GioiTinh),
                MySqlDbHelper.TaoThamSo("@dienThoai", sv.DienThoai),
                MySqlDbHelper.TaoThamSo("@email", sv.Email),
                MySqlDbHelper.TaoThamSo("@maLop", sv.MaLop),
                new MySqlParameter("@diem", MySqlDbType.Decimal) { Value = (decimal)sv.Diem },
                MySqlDbHelper.TaoThamSo("@trangThai", sv.TrangThai));
        }

        /// <summary>Cập nhật 1 sinh viên. Trả về false nếu không tìm thấy mã SV.</summary>
        public bool UpdateSinhVien(SinhVien sv)
        {
            int soDong = db.ExecuteNonQuery(
                @"UPDATE SinhVien
                  SET HoTen = @hoTen, NgaySinh = @ngaySinh, GioiTinh = @gioiTinh,
                      DienThoai = @dienThoai, Email = @email, MaLop = @maLop,
                      Diem = @diem, TrangThai = @trangThai
                  WHERE MaSV = @maSV",
                MySqlDbHelper.TaoThamSo("@maSV", sv.MaSV),
                MySqlDbHelper.TaoThamSo("@hoTen", sv.HoTen),
                new MySqlParameter("@ngaySinh", MySqlDbType.Date) { Value = sv.NgaySinh.Date },
                MySqlDbHelper.TaoThamSo("@gioiTinh", sv.GioiTinh),
                MySqlDbHelper.TaoThamSo("@dienThoai", sv.DienThoai),
                MySqlDbHelper.TaoThamSo("@email", sv.Email),
                MySqlDbHelper.TaoThamSo("@maLop", sv.MaLop),
                new MySqlParameter("@diem", MySqlDbType.Decimal) { Value = (decimal)sv.Diem },
                MySqlDbHelper.TaoThamSo("@trangThai", sv.TrangThai));

            return soDong > 0;
        }

        /// <summary>Xóa 1 sinh viên. Trả về false nếu không tìm thấy mã SV.</summary>
        public bool DeleteSinhVien(string maSV)
        {
            int soDong = db.ExecuteNonQuery(
                "DELETE FROM SinhVien WHERE MaSV = @maSV",
                MySqlDbHelper.TaoThamSo("@maSV", maSV));

            return soDong > 0;
        }

        // ==================== HỖ TRỢ ĐỌC DỮ LIỆU ====================

        private const string CauLenhChon =
            @"SELECT MaSV, HoTen, NgaySinh, GioiTinh, DienThoai, Email, MaLop, Diem, TrangThai
              FROM SinhVien";

        /// <summary>Chuyển TỪNG DÒNG MySqlDataReader thành 1 Entity SinhVien.</summary>
        private static SinhVien DocSinhVien(MySqlDataReader dr)
        {
            return new SinhVien
            {
                MaSV = Convert.ToString(dr["MaSV"]) ?? string.Empty,
                HoTen = Convert.ToString(dr["HoTen"]) ?? string.Empty,
                NgaySinh = dr["NgaySinh"] == DBNull.Value
                    ? DateTime.Today
                    : Convert.ToDateTime(dr["NgaySinh"]).Date,
                GioiTinh = Convert.ToString(dr["GioiTinh"]) ?? string.Empty,
                DienThoai = ChuyenChuoi(dr["DienThoai"]),
                Email = ChuyenChuoi(dr["Email"]),
                MaLop = Convert.ToString(dr["MaLop"]) ?? string.Empty,
                Diem = dr["Diem"] == DBNull.Value ? 0 : Convert.ToDouble(dr["Diem"]),
                TrangThai = Convert.ToString(dr["TrangThai"]) ?? string.Empty
            };
        }

        /// <summary>Chạy SELECT và chuyển kết quả thành List&lt;SinhVien&gt; (đóng kết nối ngay).</summary>
        private List<SinhVien> DocDanhSach(string sql, params MySqlParameter[] thamSo)
        {
            List<SinhVien> ds = new List<SinhVien>();
            MySqlDataReader? dr = null;

            try
            {
                dr = db.ExecuteReader(sql, thamSo);
                while (dr.Read())
                    ds.Add(DocSinhVien(dr));
            }
            catch (DataAccessException) { throw; }
            catch (MySqlException ex)
            {
                throw new DataAccessException("Lỗi khi đọc bảng SinhVien: " + ex.Message, ex);
            }
            finally
            {
                dr?.Close();
                db.CloseConnection();
            }

            return ds;
        }

        private static string? ChuyenChuoi(object giaTri)
        {
            if (giaTri == null || giaTri == DBNull.Value) return null;
            string s = Convert.ToString(giaTri) ?? string.Empty;
            return string.IsNullOrWhiteSpace(s) ? null : s;
        }

        /// <summary>Thoát ký tự đặc biệt của LIKE (% _ \) để từ khóa tìm kiếm đúng nghĩa.</summary>
        private static string Escape(string tuKhoa)
        {
            return tuKhoa.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");
        }
    }
}
