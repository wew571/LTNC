using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProjectQLsinhvien.Entity
{
    /// <summary>
    /// Tiện ích kiểm tra tính hợp lệ của các Entity
    /// bằng System.ComponentModel.DataAnnotations.Validator.
    /// </summary>
    public static class KiemTraDuLieu
    {
        /// <summary>
        /// Chạy Validator.TryValidateObject trên TẤT cả thuộc tính
        /// ([Required], [StringLength], [Range], [EmailAddress], ...)
        /// và trả về danh sách các thông báo lỗi.
        /// </summary>
        public static List<string> DanhSachLoi<T>(T? doiTuong) where T : class
        {
            if (doiTuong == null)
                return new List<string> { "Không tồn tại dữ liệu để kiểm tra." };

            ValidationContext context = new ValidationContext(doiTuong);
            List<ValidationResult> ketQua = new List<ValidationResult>();

            // ★ Phương thức trọng tâm của toàn bộ phần validate
            Validator.TryValidateObject(
                doiTuong,
                context,
                ketQua,
                validateAllProperties: true);

            return ketQua
                .Where(k => !string.IsNullOrEmpty(k.ErrorMessage))
                .Select(k => k.ErrorMessage!)
                .ToList();
        }

        /// <summary>Biến thể bool + chuỗi lỗi (mỗi lỗi một dòng).</summary>
        public static bool HopLe<T>(T? doiTuong, out string thongBao) where T : class
        {
            List<string> loi = DanhSachLoi(doiTuong);
            thongBao = string.Join(System.Environment.NewLine, loi.ConvertAll(l => "- " + l));
            return loi.Count == 0;
        }
    }
}
