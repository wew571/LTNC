using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProjectQLsinhvien.Common
{
    /// <summary>
    /// Ngoại lệ NGHIỆP VỤ – tầng BUL ném lên khi dữ liệu vi phạm quy tắc
    /// (mã trùng, lớp không tồn tại, ...). Views sẽ bắt và hiện MessageBox.
    /// </summary>
    public class BusinessException : Exception
    {
        public BusinessException(string thongDiep) : base(thongDiep) { }
        public BusinessException(string thongDiep, Exception goc) : base(thongDiep, goc) { }
    }

    /// <summary>
    /// Ngoại lệ DỮ LIỆU – tầng BUL ném lên khi Entity không hợp lệ.
    /// Kèm theo DANH SÁCH LỖI (ValidationResult) để Views hiển thị ErrorProvider
    /// cho TỪNG trường dữ liệu tương ứng.
    /// </summary>
    public class ValidationException : Exception
    {
        /// <summary>Danh sách lỗi chi tiết (mỗi lỗi biết rõ tên trường vi phạm).</summary>
        public List<ValidationResult> Loi { get; }

        public ValidationException(List<ValidationResult> loi)
            : base("Dữ liệu không hợp lệ:\n" +
                   string.Join("\n", loi.Select(l => "- " + l.ErrorMessage)))
        {
            Loi = loi ?? new List<ValidationResult>();
        }
    }

    /// <summary>
    /// Ngoại lệ TRUY VẤN DỮ LIỆU – tầng DAL ném lên khi kết nối / thực thi SQL
    /// gặp sự cố (sai kết nối, sai cú pháp, ràng buộc khóa ngoại, ...).
    /// Views sẽ bắt và hiện MessageBox.
    /// </summary>
    public class DataAccessException : Exception
    {
        public DataAccessException(string thongDiep) : base(thongDiep) { }
        public DataAccessException(string thongDiep, Exception goc) : base(thongDiep, goc) { }
    }
}
