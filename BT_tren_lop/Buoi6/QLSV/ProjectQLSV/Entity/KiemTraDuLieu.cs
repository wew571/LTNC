using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProjectQLsinhvien.Entity
{
    /// <summary>
    /// Tiện ích kiểm tra tính hợp lệ của các Entity
    /// bằng System.ComponentModel.DataAnnotations.Validator.
    /// Trả về List&lt;ValidationResult&gt; – mỗi lỗi BIẾT RÕ TÊN TRƯỜNG vi phạm
    /// (MemberNames) nên tầng Views có thể gắn ErrorProvider đúng vào điều khiển.
    /// </summary>
    public static class KiemTraDuLieu
    {
        /// <summary>
        /// Chạy Validator.TryValidateObject trên TẤT cả thuộc tính
        /// ([Required], [StringLength], [Range], [EmailAddress], ...)
        /// và trả về danh sách lỗi kèm tên trường.
        /// ★ Đây là phương thức trọng tâm của toàn bộ phần validate.
        /// </summary>
        public static List<ValidationResult> KiemTra<T>(T? doiTuong) where T : class
        {
            List<ValidationResult> ketQua = new List<ValidationResult>();

            if (doiTuong == null)
            {
                ketQua.Add(new ValidationResult("Không tồn tại dữ liệu để kiểm tra."));
                return ketQua;
            }

            ValidationContext context = new ValidationContext(doiTuong);

            Validator.TryValidateObject(
                doiTuong,
                context,
                ketQua,
                validateAllProperties: true);

            return ketQua
                .Where(k => !string.IsNullOrEmpty(k.ErrorMessage))
                .ToList();
        }

        /// <summary>Biến thể chỉ trả về CHUỖI thông báo lỗi (mỗi lỗi một dòng).</summary>
        public static List<string> DanhSachLoi<T>(T? doiTuong) where T : class
        {
            return KiemTra(doiTuong)
                .Select(k => k.ErrorMessage ?? string.Empty)
                .ToList();
        }

        /// <summary>Biến thể bool + chuỗi lỗi (mỗi lỗi một dòng, dùng cho MessageBox).</summary>
        public static bool HopLe<T>(T? doiTuong, out string thongBao) where T : class
        {
            List<string> loi = DanhSachLoi(doiTuong);
            thongBao = string.Join(System.Environment.NewLine, loi.ConvertAll(l => "- " + l));
            return loi.Count == 0;
        }
    }
}
