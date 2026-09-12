using System;

namespace QuanLyHinhHoc
{
    public class HinhChuNhat : IHinh
    {
        private double _chieuDai;
        private double _chieuRong;

        // Thuộc tính Chiều dài có kiểm tra hợp lệ
        public double ChieuDai
        {
            get { return _chieuDai; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Chiều dài phải lớn hơn 0.");
                }
                _chieuDai = value;
            }
        }

        // Thuộc tính Chiều rộng có kiểm tra hợp lệ
        public double ChieuRong
        {
            get { return _chieuRong; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Chiều rộng phải lớn hơn 0.");
                }
                _chieuRong = value;
            }
        }

        // Phương thức khởi tạo không tham số
        public HinhChuNhat()
        {
            _chieuDai = 1.0;
            _chieuRong = 1.0;
        }

        // Phương thức khởi tạo có tham số
        public HinhChuNhat(double chieuDai, double chieuRong)
        {
            ChieuDai = chieuDai;
            ChieuRong = chieuRong;
        }

        public double GetChuVi()
        {
            return 2 * (_chieuDai + _chieuRong);
        }

        public double GetDienTich()
        {
            return _chieuDai * _chieuRong;
        }

        public void Nhap()
        {
            while (true)
            {
                try
                {
                    Console.Write("Nhập chiều dài hình chữ nhật: ");
                    ChieuDai = double.Parse(Console.ReadLine() ?? "0");

                    Console.Write("Nhập chiều rộng hình chữ nhật: ");
                    ChieuRong = double.Parse(Console.ReadLine() ?? "0");

                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[Lỗi] " + ex.Message + " Vui lòng nhập lại!");
                }
            }
        }

        public void HienThi()
        {
            Console.WriteLine(string.Format("[Hình Chữ Nhật] Dài: {0:F2}, Rộng: {1:F2} | Chu vi: {2:F2} | Diện tích: {3:F2}",
                _chieuDai, _chieuRong, GetChuVi(), GetDienTich()));
        }
    }
}
