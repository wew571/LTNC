using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectQLsinhvien.Entity
{
    /// <summary>
    /// TẦNG ENTITY (dữ liệu) – lớp mô tả SINH VIÊN.
    /// Quan hệ nhiều - 1: nhiều sinh viên thuộc 1 lớp học.
    /// Chỉ chứa dữ liệu + Data Annotation để tự validate.
    /// </summary>
    public class SinhVien
    {
        [Key]
        [Required(ErrorMessage = "Vui lòng nhập Mã sinh viên.")]
        [StringLength(10, MinimumLength = 3,
            ErrorMessage = "Mã sinh viên phải có từ 3 đến 10 ký tự.")]
        public string MaSV { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập Họ và tên.")]
        [StringLength(100, MinimumLength = 5,
            ErrorMessage = "Họ và tên phải có từ 5 đến 100 ký tự.")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn Ngày sinh.")]
        public DateTime NgaySinh { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Vui lòng chọn Giới tính (Nam / Nữ).")]
        [RegularExpression("^(Nam|Nữ)$",
            ErrorMessage = "Giới tính chỉ được là \"Nam\" hoặc \"Nữ\".")]
        public string GioiTinh { get; set; } = string.Empty;

        [RegularExpression(@"^[0-9]{10}$",
            ErrorMessage = "Số điện thoại phải gồm đúng 10 chữ số.")]
        public string? DienThoai { get; set; }

        [EmailAddress(ErrorMessage = "Địa chỉ Email không hợp lệ.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn Lớp học.")]
        [StringLength(10, MinimumLength = 3,
            ErrorMessage = "Mã lớp phải có từ 3 đến 10 ký tự.")]
        public string MaLop { get; set; } = string.Empty;

        [Range(0.0, 10.0, ErrorMessage = "Điểm phải nằm trong khoảng từ 0 đến 10.")]
        public double Diem { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn Trạng thái.")]
        public string TrangThai { get; set; } = string.Empty;

        // ----- Quan hệ nhiều - 1: nhiều sinh viên – 1 lớp -----
        public LopHoc? Lop { get; set; }

        public override string ToString() => $"{MaSV} - {HoTen}";

        // ==================== TỰ KIỂM TRA TÍNH HỢP LỆ ====================

        /// <summary>
        /// Trả về DANH SÁCH LỖI (rỗng nếu hợp lệ).
        /// Mỗi lỗi kèm TÊN TRƯỜNG (ValidationResult.MemberNames) để Views
        /// gắn ErrorProvider đúng vào điều khiển tương ứng.
        /// Nguyên tắc: Data Annotation chạy trước, cộng thêm quy tắc đặc thù.
        /// Đây là API mà tầng BUL (SinhVienBUL.ThemSinhVien) đang gọi.
        /// </summary>
        public List<ValidationResult> IsInValid()
        {
            // 1. Chạy toàn bộ Data Annotation qua Validator.TryValidateObject
            List<ValidationResult> loi = KiemTraDuLieu.KiemTra(this);

            // 2. Các quy tắc không biểu diễn được bằng Attribute
            if (NgaySinh > DateTime.Today)
                loi.Add(new ValidationResult(
                    "Ngày sinh không được là ngày trong tương lai.",
                    new[] { nameof(NgaySinh) }));

            return loi;
        }

        /// <summary>Biến thể bool + chuỗi thông báo (dùng cho MessageBox).</summary>
        public bool KiemTraHopLe(out string thongBao)
        {
            List<ValidationResult> loi = IsInValid();
            thongBao = string.Join(System.Environment.NewLine,
                                   loi.ConvertAll(l => "- " + l.ErrorMessage));
            return loi.Count == 0;
        }

        /// <summary>Biến thể bool + danh sách lỗi chi tiết (dùng cho ErrorProvider).</summary>
        public bool KiemTraHopLe(out List<ValidationResult> loi)
        {
            loi = IsInValid();
            return loi.Count == 0;
        }
    }
}
