using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectQLsinhvien.Entity
{
    /// <summary>
    /// TẦNG ENTITY (dữ liệu) – lớp mô tả LỚP HỌC.
    /// Quan hệ 1 - nhiều: 1 lớp học có nhiều sinh viên.
    /// Chỉ chứa dữ liệu + Data Annotation để tự validate.
    /// </summary>
    public class LopHoc
    {
        [Key]
        [Required(ErrorMessage = "Vui lòng nhập Mã lớp.")]
        [StringLength(10, MinimumLength = 3,
            ErrorMessage = "Mã lớp phải có từ 3 đến 10 ký tự.")]
        public string MaLop { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập Tên lớp.")]
        [StringLength(100, MinimumLength = 5,
            ErrorMessage = "Tên lớp phải có từ 5 đến 100 ký tự.")]
        public string TenLop { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Ghi chú tối đa 200 ký tự.")]
        public string? GhiChu { get; set; }

        // ----- Quan hệ 1 - nhiều: 1 lớp – NHIỀU sinh viên -----
        public List<SinhVien> DanhSachSinhVien { get; set; } = new List<SinhVien>();

        public override string ToString() => TenLop;

        /// <summary>
        /// Trả về DANH SÁCH LỖI kèm TÊN TRƯỜNG (rỗng nếu hợp lệ).
        /// Đây là API mà tầng BUL (LopBUL / SinhVienBUL) đang gọi.
        /// </summary>
        public List<ValidationResult> IsInValid()
        {
            return KiemTraDuLieu.KiemTra(this);
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
