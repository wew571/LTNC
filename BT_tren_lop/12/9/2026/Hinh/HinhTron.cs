using System;

namespace QuanLyHinhHoc
{
    public class HinhTron : IHinh
    {
        private double _banKinh;

        // Thuộc tính có kiểm tra điều kiện hợp lệ
        public double BanKinh
        {
            get { return _banKinh; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Bán kính phải lớn hơn 0.");
                }
                _banKinh = value;
            }
        }

        // Phương thức khởi tạo không tham số
        public HinhTron()
        {
            _banKinh = 1.0;
        }

        // Phương thức khởi tạo có tham số
        public HinhTron(double banKinh)
        {
            BanKinh = banKinh;
        }

        public double GetChuVi()
        {
            return 2 * Math.PI * _banKinh;
        }

        public double GetDienTich()
        {
            return Math.PI * _banKinh * _banKinh;
        }

        public void Nhap()
        {
            while (true)
            {
                try
                {
                    Console.Write("Nhập bán kính hình tròn: ");
                    BanKinh = double.Parse(Console.ReadLine() ?? "0");
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
            Console.WriteLine(string.Format("[Hình Tròn] Bán kính: {0:F2} | Chu vi: {1:F2} | Diện tích: {2:F2}",
                _banKinh, GetChuVi(), GetDienTich()));
        }
    }
}
