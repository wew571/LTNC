using System;
using System.Data;
using MySql.Data.MySqlClient;
using ProjectQLsinhvien.Common;

namespace ProjectQLsinhvien.Data.DB
{
    /// <summary>
    /// TẦNG DAL – LỚP HỖ TRỢ KẾT NỐI MySQL (MySQL Workbench 8.0 CE).
    /// Cung cấp các thao tác THUẦN với cơ sở dữ liệu:
    ///   • ExecuteNonQuery (INSERT / UPDATE / DELETE)
    ///   • ExecuteReader  (SELECT đọc từng dòng)
    ///   • GetDataTable   (SELECT nạp vào DataTable)
    ///   • ExecuteScalar  (SELECT lấy 1 giá trị: COUNT(*), MAX(...))
    /// Mọi câu lệnh đều dùng THAM SỐ (@param) để tránh lỗi cú pháp và SQL Injection.
    /// Mọi lỗi kết nối / truy vấn đều được bọc lại thành DataAccessException
    /// rồi ném lên cho tầng BUL / Views xử lý.
    /// </summary>
    public class MySqlDbHelper : IDisposable
    {
        private static readonly string ConnectionString =
            "Server=127.0.0.1;Port=3306;Database=QLSV;Uid=root;Pwd=phuongTP1206@;" +
            "Charset=utf8mb4;AllowUserVariables=true";

        private MySqlConnection? con;

        public MySqlDbHelper()
        {
            con = new MySqlConnection(ConnectionString);
        }

        // ==================== MỞ / ĐÓNG KẾT NỐI ====================

        public void OpenConnection()
        {
            try
            {
                if (con == null)
                    con = new MySqlConnection(ConnectionString);

                if (con.State == ConnectionState.Closed)
                    con.Open();
            }
            catch (MySqlException ex)
            {
                throw new DataAccessException(
                    "Không thể kết nối đến cơ sở dữ liệu MySQL (QLSV): " + ex.Message, ex);
            }
        }

        public void CloseConnection()
        {
            if (con != null && con.State == ConnectionState.Open)
                con.Close();
        }

        // ==================== CÁC PHƯƠNG THỨC TRUY VẤN ====================

        /// <summary>Thực thi INSERT / UPDATE / DELETE. Trả về số dòng bị ảnh hưởng.</summary>
        public int ExecuteNonQuery(string sql, params MySqlParameter[] thamSo)
        {
            try
            {
                OpenConnection();
                using (MySqlCommand cmd = TaoCommand(sql, thamSo))
                {
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (DataAccessException) { throw; }
            catch (MySqlException ex)
            {
                throw new DataAccessException("Lỗi khi thực thi câu lệnh dữ liệu: " + ex.Message, ex);
            }
            finally
            {
                CloseConnection();
            }
        }

        /// <summary>
        /// Thực thi SELECT trả về MySqlDataReader.
        /// NGƯỜI GỌI phải tự: đọc xong → reader.Close() → CloseConnection().
        /// </summary>
        public MySqlDataReader ExecuteReader(string sql, params MySqlParameter[] thamSo)
        {
            try
            {
                OpenConnection();
                MySqlCommand cmd = TaoCommand(sql, thamSo);
                return cmd.ExecuteReader();
            }
            catch (DataAccessException) { throw; }
            catch (MySqlException ex)
            {
                CloseConnection();
                throw new DataAccessException("Lỗi khi đọc dữ liệu: " + ex.Message, ex);
            }
        }

        /// <summary>Thực thi SELECT và nạp toàn bộ kết quả vào DataTable (đóng kết nối ngay).</summary>
        public DataTable GetDataTable(string sql, params MySqlParameter[] thamSo)
        {
            try
            {
                using (MySqlConnection ketNoiMoi = new MySqlConnection(ConnectionString))
                using (MySqlCommand cmd = new MySqlCommand(sql, ketNoiMoi))
                {
                    if (thamSo != null && thamSo.Length > 0)
                        cmd.Parameters.AddRange(thamSo);

                    using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (MySqlException ex)
            {
                throw new DataAccessException("Lỗi khi nạp bảng dữ liệu: " + ex.Message, ex);
            }
        }

        /// <summary>Thực thi SELECT lấy 1 giá trị (COUNT(*), MAX(...), ...).</summary>
        public object ExecuteScalar(string sql, params MySqlParameter[] thamSo)
        {
            try
            {
                OpenConnection();
                using (MySqlCommand cmd = TaoCommand(sql, thamSo))
                {
                    object? ketQua = cmd.ExecuteScalar();
                    return ketQua ?? DBNull.Value;
                }
            }
            catch (DataAccessException) { throw; }
            catch (MySqlException ex)
            {
                throw new DataAccessException("Lỗi khi lấy giá trị tổng hợp: " + ex.Message, ex);
            }
            finally
            {
                CloseConnection();
            }
        }

        // ==================== HỖ TRỢ ====================

        private MySqlCommand TaoCommand(string sql, MySqlParameter[] thamSo)
        {
            MySqlCommand cmd = new MySqlCommand(sql, con);
            if (thamSo != null && thamSo.Length > 0)
                cmd.Parameters.AddRange(thamSo);
            return cmd;
        }

        /// <summary>
        /// Tạo tham số MySQL an toàn (giá trị null → DBNull).
        /// ★ LƯU Ý: MySql.Data suy đoán kiểu tham số số học bị SAI với giá trị 0
        /// (server nhận như NULL → WHERE trả về 0 dòng). Do đó mọi giá trị SỐ
        /// đều được khai báo TƯỜNG MINH kiểu (Decimal / Int32 / ...).
        /// </summary>
        public static MySqlParameter TaoThamSo(string ten, object? giaTri)
        {
            if (giaTri == null || giaTri == DBNull.Value)
                return new MySqlParameter(ten, DBNull.Value);

            return giaTri switch
            {
                decimal soThapPhan => new MySqlParameter(ten, MySqlDbType.Decimal)
                                       { Value = soThapPhan },
                double soDouble => new MySqlParameter(ten, MySqlDbType.Decimal)
                                    { Value = (decimal)soDouble },
                float soFloat => new MySqlParameter(ten, MySqlDbType.Decimal)
                                  { Value = (decimal)soFloat },
                int soNguyen => new MySqlParameter(ten, MySqlDbType.Int32)
                                 { Value = soNguyen },
                long soNguyenLon => new MySqlParameter(ten, MySqlDbType.Int64)
                                     { Value = soNguyenLon },
                bool dungSai => new MySqlParameter(ten, MySqlDbType.Int32)
                                 { Value = dungSai ? 1 : 0 },
                DateTime ngay => new MySqlParameter(ten, MySqlDbType.Date)
                                  { Value = ngay.Date },
                _ => new MySqlParameter(ten, giaTri)
            };
        }

        public void Dispose()
        {
            CloseConnection();
            con?.Dispose();
            con = null;
        }
    }
}
