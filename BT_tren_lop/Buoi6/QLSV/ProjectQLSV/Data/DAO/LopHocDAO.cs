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
    /// TẦNG DAL (Data Access Layer) – LỚP HỌC.
    /// Mỗi thực thể có 1 lớp DAO riêng; lớp này chỉ THUẦN thao tác với bảng LopHoc
    /// trên MySQL bằng ADO.NET:
    ///   • Thêm  → InsertLopHoc
    ///   • Sửa   → UpdateLopHoc
    ///   • Xóa   → DeleteLopHoc
    ///   • Lấy về DataList → GetAllLopHoc / GetLopByMaLop / TimKiem
    /// Không validate nghiệp vụ (việc của BUL), không đụng giao diện (việc của Views).
    /// Mọi lỗi truy vấn đều ném DataAccessException.
    /// </summary>
    public class LopHocDAO
    {
        private readonly MySqlDbHelper db = new MySqlDbHelper();

        // ==================== LẤY VỀ (SELECT) ====================

        /// <summary>Lấy toàn bộ lớp – trả về DataList&lt;LopHoc&gt;.</summary>
        public List<LopHoc> GetAllLopHoc()
        {
            return DocDanhSach("SELECT MaLop, TenLop, GhiChu FROM LopHoc ORDER BY MaLop");
        }

        /// <summary>Lấy danh sách lớp theo từ khóa (tìm kiếm PHÍA CƠ SỞ DỮ LIỆU).</summary>
        public List<LopHoc> TimKiemLop(string tuKhoa)
        {
            string tk = (tuKhoa ?? string.Empty).Trim();
            string like = "%" + Escape(tk) + "%";

            return DocDanhSach(
                @"SELECT MaLop, TenLop, GhiChu FROM LopHoc
                  WHERE @tk = '' OR MaLop LIKE @like OR TenLop LIKE @like
                  ORDER BY MaLop",
                MySqlDbHelper.TaoThamSo("@tk", tk),
                MySqlDbHelper.TaoThamSo("@like", like));
        }

        /// <summary>Tìm 1 lớp theo mã – trả về null nếu không tồn tại.</summary>
        public LopHoc? GetLopByMaLop(string maLop)
        {
            if (string.IsNullOrWhiteSpace(maLop)) return null;

            List<LopHoc> ds = DocDanhSach(
                "SELECT MaLop, TenLop, GhiChu FROM LopHoc WHERE MaLop = @maLop",
                MySqlDbHelper.TaoThamSo("@maLop", maLop.Trim()));

            return ds.Count > 0 ? ds[0] : null;
        }

        /// <summary>Kiểm tra mã lớp đã tồn tại trong CSDL chưa.</summary>
        public bool IsExist(string maLop)
        {
            if (string.IsNullOrWhiteSpace(maLop)) return false;

            object ketQua = db.ExecuteScalar(
                "SELECT COUNT(*) FROM LopHoc WHERE MaLop = @maLop",
                MySqlDbHelper.TaoThamSo("@maLop", maLop.Trim()));

            return Convert.ToInt32(ketQua) > 0;
        }

        /// <summary>Đếm số sinh viên đang thuộc lớp (giữ toàn vẹn quan hệ 1 - nhiều).</summary>
        public int DemSinhVien(string maLop)
        {
            object ketQua = db.ExecuteScalar(
                "SELECT COUNT(*) FROM SinhVien WHERE MaLop = @maLop",
                MySqlDbHelper.TaoThamSo("@maLop", maLop));

            return Convert.ToInt32(ketQua);
        }

        // ==================== THÊM / SỬA / XÓA ====================

        /// <summary>Thêm 1 lớp (INSERT). Ném DataAccessException nếu lỗi.</summary>
        public void InsertLopHoc(LopHoc lh)
        {
            db.ExecuteNonQuery(
                @"INSERT INTO LopHoc (MaLop, TenLop, GhiChu)
                  VALUES (@maLop, @tenLop, @ghiChu)",
                MySqlDbHelper.TaoThamSo("@maLop", lh.MaLop),
                MySqlDbHelper.TaoThamSo("@tenLop", lh.TenLop),
                MySqlDbHelper.TaoThamSo("@ghiChu", lh.GhiChu));
        }

        /// <summary>Cập nhật 1 lớp. Trả về false nếu không tìm thấy mã lớp.</summary>
        public bool UpdateLopHoc(LopHoc lh)
        {
            int soDong = db.ExecuteNonQuery(
                @"UPDATE LopHoc SET TenLop = @tenLop, GhiChu = @ghiChu
                  WHERE MaLop = @maLop",
                MySqlDbHelper.TaoThamSo("@tenLop", lh.TenLop),
                MySqlDbHelper.TaoThamSo("@ghiChu", lh.GhiChu),
                MySqlDbHelper.TaoThamSo("@maLop", lh.MaLop));

            return soDong > 0;
        }

        /// <summary>Xóa 1 lớp. Trả về false nếu không tìm thấy mã lớp.</summary>
        public bool DeleteLopHoc(string maLop)
        {
            int soDong = db.ExecuteNonQuery(
                "DELETE FROM LopHoc WHERE MaLop = @maLop",
                MySqlDbHelper.TaoThamSo("@maLop", maLop));

            return soDong > 0;
        }

        // ==================== HỖ TRỢ ĐỌC DỮ LIỆU ====================

        /// <summary>Chạy SELECT và chuyển TỪNG DÒNG kết quả thành List&lt;LopHoc&gt;.</summary>
        private List<LopHoc> DocDanhSach(string sql, params MySqlParameter[] thamSo)
        {
            List<LopHoc> ds = new List<LopHoc>();
            MySqlDataReader? dr = null;

            try
            {
                dr = db.ExecuteReader(sql, thamSo);
                while (dr.Read())
                {
                    LopHoc lh = new LopHoc
                    {
                        MaLop = Convert.ToString(dr["MaLop"]) ?? string.Empty,
                        TenLop = Convert.ToString(dr["TenLop"]) ?? string.Empty,
                        GhiChu = dr["GhiChu"] == DBNull.Value
                                 || string.IsNullOrWhiteSpace(Convert.ToString(dr["GhiChu"]))
                            ? null
                            : Convert.ToString(dr["GhiChu"])
                    };
                    ds.Add(lh);
                }
            }
            catch (DataAccessException) { throw; }
            catch (MySqlException ex)
            {
                throw new DataAccessException("Lỗi khi đọc bảng LopHoc: " + ex.Message, ex);
            }
            finally
            {
                dr?.Close();
                db.CloseConnection();
            }

            return ds;
        }

        /// <summary>Thoát ký tự đặc biệt của LIKE (% _ \) để từ khóa tìm kiếm đúng nghĩa.</summary>
        private static string Escape(string tuKhoa)
        {
            return tuKhoa.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");
        }
    }
}
